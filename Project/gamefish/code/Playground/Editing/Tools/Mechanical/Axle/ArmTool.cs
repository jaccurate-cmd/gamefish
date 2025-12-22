namespace Playground;

public partial class ArmTool : EditorTool
{
	[Property]
	[Title( "Arm" )]
	[Feature( EDITOR ), Group( PREFABS ), Order( PREFABS_ORDER )]
	public PrefabFile ArmPrefab { get; set; }

	[Property]
	[Title( "Attach" )]
	[Feature( EDITOR ), Group( SOUNDS ), Order( SOUNDS_ORDER )]
	public virtual SoundEvent AttachingSound { get; set; }

	/// <summary>
	/// The key you press to activate the thruster you're placing.
	/// </summary>
	[Property, InlineEditor]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public virtual ArmSettings ArmSettings { get; set; }

	public bool HasFirstPoint { get; set; }

	public ToolAttachPoint Point1 { get; set; }
	public ToolAttachPoint Point2 { get; set; }

	protected virtual void ClearPoints()
	{
		HasFirstPoint = false;

		Point1 = default;
		Point2 = default;
	}

	public override bool TryLeftClick()
	{
		if ( HasFirstPoint && Point1.IsValid() && Point2.IsValid() )
		{
			TryAttachArm();
			return true;
		}

		if ( Point1.IsValid() )
		{
			TrySetStartPoint( Point1 );
			return true;
		}

		return true;
	}

	public override void FrameSimulate( in float deltaTime )
	{
		if ( !IsClientAllowed( Client.Local ) )
			return;

		if ( !TryTrace( out var tr ) )
			return;

		if ( !tr.Hit || !tr.GameObject.IsValid() )
			return;

		// Clear Arms
		if ( PressedReload )
		{
			TryClearArms( tr.GameObject );
			ClearPoints();

			return;
		}

		if ( !Point1.IsValid() )
			HasFirstPoint = false;

		if ( !HasFirstPoint )
		{
			// First Point
			if ( ValidTarget( Client.Local, in tr ) )
				Point1 = new( tr );
		}
		else
		{
			// Second Point
			if ( ValidTarget( Client.Local, in tr ) )
				Point2 = new( tr );
		}

		if ( Point1.IsValid() )
			DrawArmGizmo( Point1 );

		if ( Point2.IsValid() )
			DrawArmGizmo( Point2 );
	}

	protected virtual void DrawArmGizmo( in ToolAttachPoint point )
	{
		if ( !point.Object.IsValid() || !point.Offset.HasValue )
			return;

		if ( !ValidAttachment( point ) )
			return;

		var c = Color.Green.WithAlpha( 0.3f );

		var tObj = point.Object.WorldTransform;
		var tArrow = tObj.ToWorld( point.Offset.Value );

		var dir = point.HitNormal ?? tArrow.Forward;

		this.DrawArrow(
			from: tArrow.Position,
			to: tArrow.Position + (dir * 6f),
			c: c, len: 0.1f, w: 5f, th: 4f,
			tWorld: global::Transform.Zero
		);
	}

	protected virtual bool TrySetStartPoint( in ToolAttachPoint point )
	{
		if ( !point.IsValid() )
		{
			Point1 = default;
			HasFirstPoint = false;

			return false;
		}

		Point1 = point;
		HasFirstPoint = true;

		return true;
	}

	protected virtual bool TryAttachArm()
	{
		if ( !IsClientAllowed( Client.Local ) )
			return false;

		if ( !Point1.IsValid() || !ValidAttachment( Point1 ) )
			return false;

		if ( !Point2.IsValid() || !ValidAttachment( Point2 ) )
			return false;

		if ( !ArmPrefab.TrySpawn( out var armObj ) )
		{
			this.Warn( $"Couldn't find/spawn {typeof( Arm )} prefab:[{ArmPrefab}]!" );
			return false;
		}

		armObj.NetworkInterpolation = false;

		if ( !armObj.Components.TryGet<Arm>( out var arm ) )
		{
			this.Warn( $"No {typeof( Arm )} on obj:[{armObj}]!" );
			armObj.Destroy();
			return false;
		}

		arm.ParentPoint = Point1;
		arm.TargetPoint = Point2;

		arm.Settings = ArmSettings;

		arm.TrySetNetworkOwner( Connection.Local, allowProxy: true );

		if ( !arm.TryAttachTo( Point1, Point2 ) )
		{
			this.Warn( $"Couldn't attach arm:[{arm}]!" );
			armObj.Destroy();
			return false;
		}

		ClearPoints();

		return true;
	}

	protected virtual bool TryClearArms( GameObject obj )
	{
		if ( !obj.IsValid() )
			return false;

		var arms = obj.Components.GetAll<Arm>( FindMode.EverythingInSelfAndDescendants );

		if ( !arms.Any( th => th.IsValid() ) )
			return false;

		RpcRemoveArms( obj );
		return true;
	}

	public virtual bool ValidTarget( Client cl, in SceneTraceResult tr )
	{
		if ( !tr.Hit || !tr.GameObject.IsValid() )
			return false;

		if ( Pawn.TryGet( tr.GameObject, out _ ) )
			return false;

		return true;
	}

	public virtual bool ValidAttachment( in ToolAttachPoint point )
	{
		if ( Pawn.TryGet( point.Object, out _ ) )
			return false;

		return true;
	}

	[Rpc.Host]
	protected void RpcRemoveArms( GameObject obj )
	{
		if ( !obj.IsValid() || !TryUse( Rpc.Caller, out _ ) )
			return;

		const FindMode findMode = FindMode.EverythingInSelf | FindMode.InDescendants;

		var arms = obj.Components.GetAll<Arm>( findMode );

		if ( !arms.Any() )
			return;

		foreach ( var th in arms.ToArray() )
			th.Destroy();
	}
}
