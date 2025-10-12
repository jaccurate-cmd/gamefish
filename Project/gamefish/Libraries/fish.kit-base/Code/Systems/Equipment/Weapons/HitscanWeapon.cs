namespace GameFish;

/// <summary>
/// 🔫 Shoots bullets.
/// </summary>
[Icon( "clear_all" )]
public partial class HitscanWeapon : BaseWeapon
{
	/// <summary>
	/// The maximum distance the bullet trace will travel.
	/// </summary>
	[Title( "Max Distance" )]
	[Property, Feature( WEAPON ), Group( BULLET )]
	public float BulletDistance { get; set; } = 4096f;

	/// <summary>
	/// The base damage of the weapon.
	/// </summary>
	[Title( "Base Damage" )]
	[Property, Feature( WEAPON ), Group( BULLET )]
	public float BulletDamage { get; set; } = 25f;

	/// <summary>
	/// Multiplies weapon damage over distance.
	/// </summary>
	[Title( "Damage Range" )]
	[Property, Feature( WEAPON ), Group( BULLET )]
	public Curve BulletDamageRange { get; set; } = new Curve( new( 0, 1 ), new( 0.2f, 1 ), new( 1, 0 ) )
	{
		TimeRange = new( 0f, 5000f ),
		ValueRange = new( 0f, 1f )
	};

	/// <summary>
	/// Scales damage based on any specified <see cref="Hitbox"/> tag.
	/// </summary>
	[Title( "Hitbox Damage" )]
	[Property, Feature( WEAPON ), Group( BULLET )]
	public Dictionary<string, float> BulletDamageScaling { get; set; } = new()
	{
		["head"] = 2f,
	};

	/// <summary>
	/// How inaccurate the bullet is.
	/// </summary>
	[Title( "Spread" )]
	[Property, Feature( WEAPON ), Group( BULLET )]
	public virtual float BulletSpread { get; set; } = 1f;

	/// <summary>
	/// Could be a cone, box, whatever.
	/// </summary>
	[Title( "Spread Shape" )]
	[Property, Feature( WEAPON ), Group( BULLET )]
	public virtual SpreadShape BulletSpreadType { get; set; }

	protected override void OnPrimary()
	{
		base.OnPrimary();

		GetBulletTrace( GetBulletTransform() );
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// A transform is used to preserve rotation of objects.
	/// </remarks>
	/// <returns> The final position and rotation of the bullet. </returns>
	public virtual Transform GetBulletTransform()
		=> new( AimPosition, AimRotation * GetSpreadConeRotation( BulletSpread ) );

	public virtual SceneTrace GetBulletTrace( in Vector3 origin, in Vector3 dir )
	{
		if ( !Owner.IsValid() )
			return default;

		return Owner.GetEyeTrace( origin, origin + (dir * BulletDistance) );
	}

	public SceneTrace GetBulletTrace( in Transform t )
		=> GetBulletTrace( t.Position, t.Rotation.Forward );
}
