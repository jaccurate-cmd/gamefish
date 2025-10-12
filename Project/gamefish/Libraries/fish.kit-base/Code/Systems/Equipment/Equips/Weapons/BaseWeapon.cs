
/// <summary>
/// 🔫 Shoots bullets and/or projectiles.
/// </summary>
[Icon( "clear_all" )]
public partial class BaseWeapon : AmmoEquip
{
	/// <summary>
	/// Spawns this weapon's projectile at the given transform.
	/// Sets the object's team if specified.
	/// </summary>
	protected virtual GameObject SpawnProjectile( PrefabFile projPrefab, in Transform t, bool setTeam = true )
	{
		if ( !projPrefab.TrySpawn( t, out var proj ) )
			return null;

		if ( setTeam && Owner?.Team is Team team )
			proj.SetTeam( team, FindMode.EverythingInSelfAndDescendants );

		OnSpawnProjectile( proj );

		return proj;
	}

	/// <summary>
	/// By default this simply network spawns the projectile.
	/// </summary>
	protected virtual void OnSpawnProjectile( GameObject proj )
	{
		if ( proj.IsValid() )
			proj.NetworkSetup( Network.Owner, NetworkOrphaned.Destroy );
	}
}
