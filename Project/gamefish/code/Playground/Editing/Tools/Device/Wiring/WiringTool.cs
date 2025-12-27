namespace Playground;

public partial class WiringTool : EditorTool
{
	public Entity TargetEntity { get; set; }
	public Vector3? TargetWorldPosition { get; set; }
	public Vector3? TargetLocalPosition { get; set; }

	public (Entity Entity, Vector3 LocalPos) Point1 { get; set; }
	public (Entity Entity, Vector3 LocalPos) Point2 { get; set; }

	public override void OnExit()
	{
		base.OnExit();

		Clear();
	}

	public override bool TryLeftClick()
		=> true;

	public override void FrameSimulate( in float deltaTime )
	{
		UpdateTarget( in deltaTime );

		if ( TargetEntity.IsValid() && TargetEntity is Device device )
			device.DrawWireGizmos();

		UpdatePlacing();
		UpdateClearing();

		DrawTargetGizmos();
	}

	protected void DrawTargetGizmos()
	{
		if ( TargetWorldPosition is not Vector3 targetPos )
			return;

		var ent1 = Point1.Entity;

		if ( !IsValidTarget( ent1 ) )
			return;

		var ent2 = IsValidTarget( Point2.Entity ) ? Point2.Entity : TargetEntity;

		if ( !ent2.IsValid() )
			return;

		var startPoint = ent1.WorldTransform.PointToWorld( Point1.LocalPos );

		var c = Color.Black.WithAlpha( 0.7f );

		if ( !CanWire( ent1, ent2 ) )
			c = c.WithAlphaMultiplied( 0.5f );

		this.DrawArrow(
			from: startPoint, to: targetPos,
			c: c, len: 7f, w: 2f, th: 4f,
			tWorld: global::Transform.Zero
		);
	}

	protected void UpdatePlacing()
	{
		if ( !PressedPrimary )
			return;

		if ( !IsValidTarget( TargetEntity ) )
			return;

		if ( TargetLocalPosition is not Vector3 localPos )
			return;

		if ( !Point1.Entity.IsValid() )
		{
			Point1 = (TargetEntity, localPos);
		}
		else
		{
			if ( TargetEntity != Point1.Entity )
				Point2 = (TargetEntity, localPos);
		}

		if ( !CanWire( Point1.Entity, Point2.Entity ) )
			return;

		RpcHostRequestWire( Point1.Entity, Point2.Entity, Point1.LocalPos, Point2.LocalPos );

		Clear();
	}

	protected void UpdateClearing()
	{
		if ( !PressedReload )
			return;

		if ( TargetEntity.IsValid() )
			RpcHostRequestClear( TargetEntity );
	}

	protected virtual void UpdateTarget( in float deltaTime )
	{
		TargetEntity = null;
		TargetWorldPosition = null;

		if ( !IsClientAllowed( Client.Local ) )
			return;

		if ( !TryTrace( out var tr ) )
			return;

		if ( !TryFindWireable( tr.GameObject, out var ent ) )
			return;

		if ( !TrySetTarget( ent, in tr ) )
			return;

		var cSphere = Color.White.WithAlpha( 0.4f );

		this.DrawSphere( 2f, tr.HitPosition, Color.Transparent, cSphere, global::Transform.Zero );
	}

	public virtual bool TryFindWireable( GameObject obj, out Entity ent )
	{
		ent = null;

		if ( !obj.IsValid() || !obj.Active )
			return false;

		const FindMode findMode = FindMode.EnabledInSelf
			| FindMode.InAncestors;

		ent = obj.Components.GetAll<Entity>( findMode )
		   .FirstOrDefault( ent => ent is IWired );

		// this.Log( ent );

		return ent.IsValid();
	}

	public bool TrySetTarget( Entity ent, in SceneTraceResult tr )
	{
		if ( !IsValidTarget( ent ) )
			return false;

		TargetEntity = ent;

		TargetWorldPosition = tr.HitPosition;
		TargetLocalPosition = ent.WorldTransform.PointToLocal( tr.HitPosition );

		return true;
	}

	public static bool CanWire( Entity ent1, Entity ent2 )
	{
		if ( ent1 == ent2 )
			return false;

		if ( ent1 is not Device && ent2 is not Device )
			return false;

		return IsValidTarget( ent1 ) && IsValidTarget( ent2 );
	}

	public static bool IsValidTarget( Entity ent )
	{
		if ( !ent.IsValid() || ent is not IWired )
			return false;

		return true;
	}

	protected void Clear()
	{
		TargetEntity = null;

		TargetWorldPosition = null;
		TargetLocalPosition = null;

		Point1 = default;
		Point2 = default;
	}

	[Rpc.Host]
	protected void RpcHostRequestWire( Entity ent1, Entity ent2, Vector3 localPos1, Vector3 localPos2 )
	{
		if ( !TryUse( Rpc.Caller, out _ ) )
			return;

		if ( !ent1.IsValid() || !ent2.IsValid() )
			return;

		if ( ent1 is Device device1 )
			device1.TryWire( ent2, localPos2 );

		if ( ent2 is Device device2 )
			device2.TryWire( ent1, localPos1 );
	}

	[Rpc.Host]
	protected void RpcHostRequestClear( Entity ent )
	{
		if ( !ent.IsValid() || ent is not Device device )
			return;

		if ( !TryUse( Rpc.Caller, out _ ) )
			return;

		device.Wires?.Clear();
	}
}
