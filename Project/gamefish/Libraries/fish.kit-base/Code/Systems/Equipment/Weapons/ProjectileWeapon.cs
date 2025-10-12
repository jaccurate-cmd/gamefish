namespace GameFish;

/// <summary>
/// 🚀 Shoots projectiles.
/// </summary>
[Icon( "rocket_launch" )]
public partial class ProjectileWeapon : BaseWeapon
{
	[Property]
	[Title( "Prefab" )]
	[Feature( WEAPON ), Group( PROJECTILE )]
	public virtual PrefabFile ProjectilePrefab { get; set; } = Prefab.GetFile( "prefabs/gamefish/kit_fps/projectiles/missile1.prefab" );

	/// <summary>
	/// The projectile's velocity relative to the owner's aim. <br />
	/// The <c>X</c> axis is forward speed.
	/// </summary>
	[Property]
	[Title( "Velocity" )]
	[Feature( WEAPON ), Group( PROJECTILE )]
	public virtual Vector3 ProjectileVelocity { get; set; } = new Vector3( 1500f, 0f, 0f );

	/// <summary>
	/// Offsets the projectile given its spawning transform is spawned by this position/rotation.
	/// </summary>
	[Property]
	[InlineEditor]
	[Title( "Spawn Offset" )]
	[Feature( WEAPON ), Group( PROJECTILE )]
	public virtual Offset ProjectileOffset { get; set; } = new( Vector3.Forward * 16f, Rotation.Identity );

	/// <summary>
	/// What to override the spawned prefab's scale with.
	/// </summary>
	[Property]
	[Title( "Scale" )]
	[Feature( WEAPON ), Group( PROJECTILE )]
	public virtual Vector3 ProjectileScale { get; set; } = Vector3.One;

	protected override void OnPrimary()
	{
		TrySpawnProjectile( out var _ );
	}

	/// <returns> The default world transform for the projectile to spawn at. </returns>
	public virtual Transform GetProjectileTransform()
	{
		return new Transform( AimPosition, AimRotation, ProjectileScale )
			.AddOffset( ProjectileOffset );
	}

	/// <returns> The velocity to apply relative to an aiming rotation. </returns>
	public virtual Vector3 GetProjectileVelocity( in Rotation rAim )
		=> ProjectileVelocity != default
			? rAim * ProjectileVelocity
			: default;

	/// <summary>
	/// Tries to spawn a projectile using the given(or default) transform.
	/// </summary>
	/// <returns> If the projectile could be spawned. </returns>
	public virtual bool TrySpawnProjectile( out GameObject proj, Transform? tProj = null )
		=> (proj = SpawnProjectile( ProjectilePrefab, tProj ?? GetProjectileTransform() )).IsValid();
}
