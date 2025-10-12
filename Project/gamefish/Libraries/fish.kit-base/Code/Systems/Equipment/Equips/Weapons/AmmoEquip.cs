using System;

namespace GameFish;

/// <summary>
/// Has ammo and can reload.
/// </summary>
public abstract partial class AmmoEquip : BaseEquip
{
	[Property]
	[Feature( WEAPON ), Group( AMMO )]
	public virtual bool HasAmmo { get; set; } = true;

	[Title( "Capacity" )]
	[ShowIf( nameof( HasAmmo ), true )]
	[Property, Feature( WEAPON ), Group( AMMO )]
	public virtual int DefaultMaxAmmo { get; set; } = 20;

	[ShowIf( nameof( HasAmmo ), true )]
	[Property, Feature( WEAPON ), Group( AMMO )]
	public float ReloadDuration { get; set; } = 1f;

	public virtual int Ammo { get => _ammo; set => _ammo = Math.Max( 0, value ); }
	private int _ammo;

	public bool Reloading { get; set; }
	public TimeUntil ReloadEnds { get; set; }

	public virtual int GetMaxAmmo() => DefaultMaxAmmo;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !IsDeployed )
			return;

		if ( Reloading )
		{
			if ( ReloadEnds )
				FinishReload();

			return;
		}
	}

	protected override void OnHolster()
	{
		base.OnHolster();

		Reloading = false;
	}

	public override bool CanPrimary()
		=> base.CanPrimary() && (!HasAmmo || !Reloading);

	public override bool CanSecondary()
		=> base.CanSecondary() && (!HasAmmo || !Reloading);

	public virtual void StartReloading()
	{
		if ( !HasAmmo || Reloading )
			return;

		ReloadEnds = ReloadDuration;
	}

	public virtual void FinishReload()
	{
		ReloadEnds = 0f;
		Ammo = GetMaxAmmo();
	}
}
