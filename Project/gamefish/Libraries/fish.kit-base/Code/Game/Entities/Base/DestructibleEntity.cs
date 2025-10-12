namespace GameFish;

/// <summary>
/// An entity that supports health and physics.
/// </summary>
public partial class DestructibleEntity : PhysicsEntity, IHealth
{
	public const string VALUES = "Values";

	[Sync]
	[Property, Feature( HEALTH )]
	public bool IsAlive { get; protected set; } = true;

	/// <summary> Is this capable of ever taking damage? </summary>
	[Property, Feature( HEALTH )]
	public virtual bool IsDestructible { get; set; } = true;

	[Sync]
	[Property, Title( "Health" )]
	[Group( VALUES ), Feature( HEALTH )]
	[ShowIf( nameof( IsDestructible ), true )]
	public float Health { get; protected set; } = 100f;

	[Sync]
	[Property, Title( "Max Health" )]
	[Group( VALUES ), Feature( HEALTH )]
	[ShowIf( nameof( IsDestructible ), true )]
	public float MaxHealth { get; set; } = 100f;

	[Property]
	[Feature( HEALTH ), Group( DEBUG )]
	[ShowIf( nameof( IsDestructible ), true )]
	public float DebugDamage { get; set; } = 25f;

	public IEnumerable<IHealthEvent> HealthEvents
		=> Components?.GetAll<IHealthEvent>( FindMode.EnabledInSelfAndDescendants ) ?? [];

	[Button]
	[Title( "Take Damage" )]
	[Feature( HEALTH ), Group( DEBUG )]
	[ShowIf( nameof( IsDestructible ), true )]
	protected void DebugTakeDamage()
		=> RpcTryDamage( new() { Damage = DebugDamage } );

	public void OnDamage( in DamageInfo dmgInfo )
		=> RpcTryDamage( dmgInfo );

	[Rpc.Owner( NetFlags.Reliable | NetFlags.HostOnly )]
	public void RpcHostSetHealth( float hp ) => SetHealth( in hp );

	[Rpc.Owner( NetFlags.Reliable | NetFlags.HostOnly )]
	public void RpcHostModifyHealth( float hp ) => ModifyHealth( in hp );

	[Rpc.Owner( NetFlags.Reliable | NetFlags.HostOnly )]
	public void RpcHostDie() => Die();

	[Rpc.Owner( NetFlags.Reliable | NetFlags.HostOnly )]
	public void RpcHostRevive() => Revive();

	[Rpc.Owner( NetFlags.Reliable )]
	public void RpcTryDamage( DamageInfo dmgInfo ) => TryDamage( dmgInfo );

	public virtual void SetHealth( in float hp )
	{
		if ( IsProxy )
			return;

		Health = hp.Clamp( 0f, MaxHealth );

		foreach ( var e in HealthEvents )
			e.OnSetHealth( hp );

		if ( !IsAlive && Health > 0 )
			Revive();
		else if ( IsAlive && Health <= 0 )
			Die();
	}

	public virtual void ModifyHealth( in float hp )
		=> SetHealth( Health + hp );

	public virtual void Die()
	{
		if ( IsProxy || !IsAlive )
			return;

		if ( Health > 0f )
			Health = 0f;

		IsAlive = false;
		OnDeath();
	}

	public virtual void Revive( bool restoreHealth = false )
	{
		if ( IsProxy || IsAlive )
			return;

		IsAlive = true;
		Health = Health.Max( restoreHealth ? MaxHealth : Health.Max( 1 ) );

		OnRevival();
	}

	public virtual void OnDeath()
	{
		if ( IsProxy )
			return;

		foreach ( var e in HealthEvents )
			e.OnDeath();
	}

	public virtual void OnRevival()
	{
		foreach ( var e in HealthEvents )
			e.OnRevival();
	}

	public virtual bool CanDamage( in DamageInfo dmgInfo )
		=> IsDestructible && dmgInfo.Damage > 0;

	public virtual bool TryDamage( DamageInfo dmgInfo )
	{
		if ( !CanDamage( in dmgInfo ) )
			return false;

		foreach ( var e in HealthEvents )
			if ( !e.TryDamage( ref dmgInfo ) )
				return false;

		ApplyDamage( dmgInfo );
		return true;
	}

	public virtual void ApplyDamage( DamageInfo dmgInfo )
	{
		foreach ( var e in HealthEvents )
			e.OnApplyDamage( ref dmgInfo );

		ModifyHealth( -dmgInfo.Damage );
	}
}
