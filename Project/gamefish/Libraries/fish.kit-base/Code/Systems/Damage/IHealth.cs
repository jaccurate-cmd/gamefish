using System;

namespace GameFish;

public interface IHealth : Component.IDamageable
{
	public bool IsAlive { get; }
	public float Health { get; }

	/// <summary>
	/// The collection of <see cref="IHealthEvent"/>s relevant to this object. <br />
	/// Example: retrieved from a <see cref="ComponentList"/>.
	/// </summary>
	public IEnumerable<IHealthEvent> HealthEvents { get; }

	/// <summary>
	/// The engine's method for dealing damage that does not return success.
	/// </summary>
	void Component.IDamageable.OnDamage( in DamageInfo dmgInfo )
		=> TryDamage( dmgInfo );

	public void SetHealth( in float hp );
	public void ModifyHealth( in float hp );

	public bool CanDamage( in DamageInfo dmgInfo );
	public bool TryDamage( DamageInfo dmgInfo );
}
