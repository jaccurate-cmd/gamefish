namespace GameFish;

/// <summary>
/// An entity that supports health and physics.
/// </summary>
public partial class DestructibleEntity : PhysicsEntity
{
	public const string HEALTH = "💖 Health";

	public const string GROUP_VALUES = "Values";

	/// <summary>
	/// Does this entity have a valid <see cref="global::GameFish.HealthComponent"/>?
	/// </summary>
	public bool HasHealth => HealthComponent.IsValid();

	/// <summary>
	/// The <see cref="global::GameFish.HealthComponent"/> in this object hierarchy.
	/// Add one to allow taking damage, healing, dying etc.
	/// </summary>
	[Title( "Component" )]
	[Property, Feature( ENTITY ), Group( HEALTH )]
	public HealthComponent HealthComponent
	{
		get => _hp.IsValid() ? _hp
			: _hp = Components?.Get<HealthComponent>( FindMode.EverythingInSelf | FindMode.InDescendants | FindMode.InAncestors );

		set { _hp = value; }
	}

	protected HealthComponent _hp;

	[ShowIf( nameof( HasHealth ), true )]
	[Property, Feature( ENTITY ), Group( HEALTH )]
	public bool IsAlive => HealthComponent?.IsAlive ?? false;

	[ShowIf( nameof( HasHealth ), true )]
	[Property, Feature( ENTITY ), Group( HEALTH )]
	public bool IsDestructible => HealthComponent?.IsDestructible ?? false;

	[Title( "Initial" )]
	[ShowIf( nameof( HasHealth ), true )]
	[Property, Feature( ENTITY ), Group( HEALTH )]
	public float Health => HealthComponent?.Health ?? 0f;

	[Title( "Max" )]
	[ShowIf( nameof( HasHealth ), true )]
	[Property, Feature( ENTITY ), Group( HEALTH )]
	public float MaxHealth => HealthComponent?.MaxHealth ?? 0f;
}
