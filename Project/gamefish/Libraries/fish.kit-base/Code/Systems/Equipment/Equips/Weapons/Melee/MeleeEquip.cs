namespace GameFish;

public partial class MeleeEquip : BaseEquip
{
	/// <summary>
	/// Should the owner's scale affect their attack distance?
	/// </summary>
	[Title( "Scale Distance" )]
	[Property, Feature( MELEE )]
	public bool IsAttackDistanceScaled { get; set; } = true;

	/// <summary>
	/// By default this is <c>UsableRange.Max</c>.
	/// </summary>
	[Property, Feature( MELEE )]
	public virtual float AttackDistance => UsableRange.Max.Positive();

	/// <summary>
	/// The radius of the sphere used when tracing.
	/// </summary>
	[Property, Feature( MELEE )]
	public float AttackRadius { get; set; } = 8f;
	[Property, Feature( MELEE )]
	public float AttackDamage { get; set; } = 20f;

	[Property, Feature( MELEE ), Group( PHYSICS )]
	public float AttackKnockback { get; set; } = 0f;

	[Property, Feature( MELEE ), Group( SOUNDS )]
	public SoundEvent AttackHitSound { get; set; }
	[Property, Feature( MELEE ), Group( SOUNDS )]
	public SoundEvent AttackMissSound { get; set; }
	[Property, Feature( MELEE ), Group( SOUNDS )]
	public PrefabFile AttackEffectPrefab { get; set; }

	public virtual float GetAttackDistance()
	{
		// Factor in radius of sphere.
		var attackDist = AttackDistance;

		if ( IsAttackDistanceScaled )
		{
			var scale = Owner?.WorldScale.x.Abs() ?? 1f;
			attackDist = (attackDist * scale).Max( 0f );
		}

		return attackDist.Positive();
	}

	protected override void OnPrimary()
	{
		OnMeleeAttack( AimRotation );
	}

	public virtual void OnMeleeAttack( in Rotation rAim )
	{
		if ( !Scene.IsValid() || !Owner.IsValid() )
			return;

		var eyeTrace = Owner.GetEyeTrace( distance: GetAttackDistance(), dir: rAim.Forward )
			.WithoutTags( Owner.TeamTag ?? string.Empty );

		var tr = AttackRadius > 0f
			? eyeTrace.Radius( AttackRadius ).Run()
			: eyeTrace.Run();

		if ( !tr.Hit || tr.GameObject is not GameObject obj )
			return;

		if ( Owner.GetRelationship( obj ) is not Relationship.Enemy )
		{
			// this.Log( obj, " is not enemy. relationship: " + Owner.GetRelationship( obj ) );
			return;
		}

		var tags = new TagSet( [DamageTags.MELEE] );

		var dmgInfo = new DamageInfo( AttackDamage, Owner.GameObject, GameObject )
		{ Tags = tags };

		// Attempt to deal damage and play appropriate sound.
		if ( obj.TryDamage( dmgInfo ) )
		{
			if ( AttackKnockback != 0f )
			{
				if ( obj.Components.TryGet<Rigidbody>( out var rb, FindMode.EnabledInSelf | FindMode.InAncestors ) )
					rb.Velocity += tr.Direction * AttackKnockback;
				else if ( obj.Components.TryGet<IVelocity>( out var vel, FindMode.EnabledInSelf | FindMode.InAncestors ) )
					vel.TryModifyVelocity( tr.Direction * AttackKnockback );
			}

			// Blood effect or something.
			AttackEffectPrefab.TrySpawn( tr.HitPosition, out _ );

			if ( AttackHitSound.IsValid() )
				Sound.Play( AttackHitSound, tr.HitPosition );
		}
		else
		{
			if ( AttackMissSound.IsValid() )
				Sound.Play( AttackMissSound, tr.HitPosition );
		}
	}
}
