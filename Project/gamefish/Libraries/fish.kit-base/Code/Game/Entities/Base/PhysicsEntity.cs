namespace GameFish;

/// <summary>
/// An entity that can have a <see cref="Sandbox.Rigidbody"/>.
/// </summary>
public partial class PhysicsEntity : ModuleEntity, IPhysics
{
	protected const int PHYSICS_ORDER = -9001;

	protected Rigidbody _rb;
	public Rigidbody Rigidbody => _rb.IsValid() ? _rb
		: Components?.Get<Rigidbody>( FindMode.EverythingInSelf | FindMode.InAncestors );

	public PhysicsBody PhysicsBody => Rigidbody?.PhysicsBody;
	public Vector3 MassCenter => PhysicsBody?.MassCenter ?? GetPosition();

	/// <summary>
	/// By default this is the velocity of the Rigidbody(if any, otherwise zero).
	/// It could however also be the velocity of some other component.
	/// </summary>
	[Property]
	[Feature( ENTITY ), Group( PHYSICS ), Order( PHYSICS_ORDER )]
	public virtual Vector3 Velocity
	{
		get => Rigidbody?.Velocity ?? Vector3.Zero;
		set
		{
			if ( Rigidbody.IsValid() )
				Rigidbody.Velocity = value;
		}
	}

	public override bool TryTeleport( in Transform tWorld )
	{
		WorldPosition = tWorld.Position;
		WorldRotation = tWorld.Rotation;

		return !IsProxy;
	}
}
