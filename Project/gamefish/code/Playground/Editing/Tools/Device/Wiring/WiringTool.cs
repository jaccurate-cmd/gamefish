namespace Playground;

public partial class WiringTool : EditorTool
{
	public Vector3? TargetLocalPosition { get; protected set; }

	public (Component Parent, Vector3 LocalPos) Point1 { get; protected set; }
	public (Component Parent, Vector3 LocalPos) Point2 { get; protected set; }

	public override void OnExit()
	{
		base.OnExit();

		Clear();
	}

	public override void FrameSimulate( in float deltaTime )
	{
		base.FrameSimulate( in deltaTime );

		if ( !IsMenuOpen )
		{
			if ( TargetComponent.IsValid() && TargetComponent is Device device )
				device.RenderHelpers();
		}
	}

	protected override void RenderHelpers()
	{
		base.RenderHelpers();

		RenderWireHelper();
	}

	protected void RenderWireHelper()
	{
		if ( TargetTrace is not SceneTraceResult tr )
			return;

		var ent1 = Point1.Parent;

		if ( !IsValidTarget( ent1 ) )
			return;

		var ent2 = IsValidTarget( Point2.Parent ) ? Point2.Parent : TargetComponent as Entity;

		if ( !ent2.IsValid() )
			return;

		var startPoint = ent1.WorldTransform.PointToWorld( Point1.LocalPos );

		var c = Color.Black.WithAlpha( 0.7f );

		if ( !CanWire( ent1, ent2 ) )
			c = c.WithAlphaMultiplied( 0.5f );

		this.DrawArrow(
			from: startPoint, to: tr.EndPosition,
			c: c, len: 7f, w: 2f, th: 4f,
			tWorld: global::Transform.Zero
		);
	}

	protected override void OnPrimary( in SceneTraceResult tr )
	{
		if ( !IsValidTarget( TargetComponent ) )
			return;

		if ( TargetLocalPosition is not Vector3 localPos )
			return;

		if ( !Point1.Parent.IsValid() )
		{
			Point1 = (TargetComponent, localPos);
			Point2 = default;

			return;
		}

		if ( TargetComponent != Point1.Parent )
			Point2 = (TargetComponent, localPos);

		if ( !CanWire( Point1.Parent, Point2.Parent ) )
			return;

		RpcHostRequestWire( Point1.Parent, Point2.Parent, Point1.LocalPos, Point2.LocalPos );

		Clear();
	}


	protected override void OnReload( in SceneTraceResult tr )
	{
		base.OnReload( tr );

		if ( TargetComponent is Entity ent )
			RpcHostRequestClear( ent );
	}

	protected override void Clear()
	{
		base.Clear();

		Point1 = default;
		Point2 = default;
	}

	protected override void ClearTarget()
	{
		base.ClearTarget();

		TargetLocalPosition = null;
	}

	public override bool IsValidTarget( Component c )
		=> base.IsValidTarget( c ) && c is IWired;

	public override bool TrySetTarget( in SceneTraceResult tr, Component target )
	{
		if ( !base.TrySetTarget( in tr, target ) )
			return false;

		TargetLocalPosition = target.WorldTransform.PointToLocal( tr.HitPosition );

		return true;
	}

	public bool CanWire( Component ent1, Component ent2 )
	{
		if ( ent1 == ent2 )
			return false;

		if ( ent1 is not Device && ent2 is not Device )
			return false;

		return IsValidTarget( ent1 ) && IsValidTarget( ent2 );
	}

	[Rpc.Host]
	protected void RpcHostRequestWire( Component ent1, Component ent2, Vector3 localPos1, Vector3 localPos2 )
	{
		if ( !TryUse( Rpc.Caller, out _ ) )
			return;

		if ( !IsValidTarget( ent1 ) || !IsValidTarget( ent2 ) )
			return;

		if ( ent1 is Device device1 )
			device1.TryWire( ent2 as Entity, localPos2 );

		if ( ent2 is Device device2 )
			device2.TryWire( ent1 as Entity, localPos1 );
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
