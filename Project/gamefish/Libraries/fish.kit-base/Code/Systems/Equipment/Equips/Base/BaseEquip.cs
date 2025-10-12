namespace GameFish;

/// <summary>
/// An <see cref="PawnEquipment"/> can store, deploy and use this.
/// </summary>
[EditorHandle( Icon = "🏹" )]
public abstract partial class BaseEquip : PhysicsEntity, ISkinned, Component.ExecuteInEditor
{
	protected const string SLOT = "Slot";
	protected const string RANGE = "Range";
	protected const string MODELS = "Models";
	protected const string INPUT_METHOD = "Input Method";

	[Property]
	[Feature( EQUIP ), Group( MODELS )]
	public Model ViewModel { get; set; }

	[Property]
	[Feature( EQUIP ), Group( MODELS )]
	public Model WorldModel { get => WorldRenderer?.Model; set { if ( WorldRenderer.IsValid() ) WorldRenderer.Model = value; } }

	[Property]
	[Feature( EQUIP ), Group( MODELS )]
	public SkinnedModelRenderer WorldRenderer
	{
		// Auto-cache the component.
		get => _wr.IsValid() ? _wr
			: _wr = Components?.Get<SkinnedModelRenderer>( FindMode.EverythingInDescendants );

		set { _wr = value; }
	}

	protected SkinnedModelRenderer _wr;

	public SkinnedModelRenderer SkinRenderer { get => WorldRenderer; set => _wr = value; }

	/// <summary>
	/// The slot this is meant to go in.
	/// </summary>
	[Property]
	[Title( "Default" )]
	[Feature( EQUIP ), Group( SLOT )]
	public EquipSlot DefaultSlot { get; set; }

	[Sync]
	[Property]
	[Title( "Current" )]
	[Feature( EQUIP ), Group( SLOT )]
	[ShowIf( nameof( InGame ), true )]
	public int Slot { get; set; }

	[Property, Feature( INPUT ), Group( INPUT_METHOD )]
	public virtual bool PrimaryHeld { get; set; }
	public virtual bool PrimaryInput => PrimaryHeld ? Input.Down( PrimaryAction ) : Input.Pressed( PrimaryAction );

	[Property, InputAction]
	[Feature( INPUT ), Group( INPUT_METHOD )]
	public string PrimaryAction { get; set; } = "Attack1";

	[Property, Feature( INPUT ), Group( INPUT_METHOD )]
	public virtual bool SecondaryHeld { get; set; }
	public virtual bool SecondaryInput => SecondaryHeld ? Input.Down( SecondaryAction ) : Input.Pressed( SecondaryAction );

	[Property, InputAction]
	[Feature( INPUT ), Group( INPUT_METHOD )]
	public string SecondaryAction { get; set; } = "Attack2";

	[Property, Feature( INPUT ), Group( TIMING )]
	public virtual float PrimaryCooldown { get; set; } = 0.5f;
	public virtual TimeSince? LastPrimary { get; set; }

	[Property, Feature( INPUT ), Group( TIMING )]
	public virtual float SecondaryCooldown { get; set; } = 1.0f;
	public virtual TimeSince? LastSecondary { get; set; }

	public virtual Vector3 AimPosition => Owner?.EyePosition ?? WorldPosition;
	public virtual Rotation AimRotation => Owner?.EyeRotation ?? Rotation.Identity;

	public Transform AimTransform => new( AimPosition, AimRotation );
	public Vector3 AimDirection => AimRotation.Forward;

	public override string ToString()
		=> $"{ClassId}|{ClassName}";

	protected override void OnStart()
	{
		base.OnStart();

		Tags?.Add( TAG_EQUIP );
	}

	public virtual bool AllowInput()
	{
		if ( !IsDeployed )
			return false;

		var owner = Owner;

		if ( !owner.IsValid() || !owner.IsAlive )
			return false;

		return owner.AllowInput();
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		if ( !AllowInput() )
			return;

		if ( PrimaryInput )
			TryPrimary();

		if ( SecondaryInput )
			TrySecondary();
	}

	/// <returns> If a pawn can equip this. </returns>
	public virtual bool AllowEquip( BasePawn pawn )
	{
		return pawn.IsValid() && pawn.HasModule<PawnEquipment>();
	}

	public virtual bool CanPrimary()
		=> !LastPrimary.HasValue || LastPrimary >= PrimaryCooldown;

	public virtual bool CanSecondary()
		=> !LastSecondary.HasValue || LastSecondary >= SecondaryCooldown;

	public virtual bool TryPrimary()
	{
		if ( CanPrimary() )
		{
			LastPrimary = 0;
			OnPrimary();

			return true;
		}

		return false;
	}

	/// <summary>
	/// Executes the primary function of this equipment. <br />
	/// You should call <see cref="TryPrimary"/> to call this while respecting cooldowns.
	/// </summary>
	protected virtual void OnPrimary()
	{
	}

	public virtual bool TrySecondary()
	{
		if ( CanSecondary() )
		{
			LastSecondary = 0;
			OnSecondary();

			return true;
		}

		return false;
	}

	/// <summary>
	/// Executes the secondary function of this equipment. <br />
	/// You should call <see cref="TrySecondary"/> to call this while respecting cooldowns.
	/// </summary>
	protected virtual void OnSecondary()
	{
	}
}
