using GameFish;

namespace Playground;

public partial class LeafblowerPrimary : EditorToolFunction
{
	[Property]
	[Title( "Force" )]
	[Feature( EDITOR ), Group( FORCES ), Order( EDITOR_ORDER )]
	public float PushForce { get; set; } = 500f;

	[Property]
	[Feature( EDITOR ), Group( FORCES ), Order( EDITOR_ORDER )]
	public float PushDelay { get; set; } = 0.1f;

	public override void Activate( in SceneTraceResult tr )
	{
		var vel = tr.Direction * PushForce;
		vel *= PushDelay.Max( Time.Delta );

		const FindMode findMode = FindMode.EnabledInSelf | FindMode.InAncestors;

		if ( tr.GameObject.Components.TryGet<IVelocity>( out var iVel, findMode ) )
			iVel.TryImpulse( vel );
		else if ( tr.GameObject.Components.TryGet<Rigidbody>( out var rb, findMode ) )
			rb.Velocity += vel;
	}
}
