namespace Playground;

partial class Editor
{
	[Rpc.Broadcast( NetFlags.UnreliableNoDelay )]
	public virtual void BroadcastImpulse( GameObject obj, Vector3 vel )
	{
		if ( !obj.IsValid() )
			return;

		const FindMode findMode = FindMode.EnabledInSelf | FindMode.InAncestors;

		if ( obj.Components.TryGet<IVelocity>( out var iVel, findMode ) )
			iVel.Velocity += vel;
		else if ( obj.Components.TryGet<Rigidbody>( out var rb, findMode ) )
			rb.Velocity += vel;
	}
}
