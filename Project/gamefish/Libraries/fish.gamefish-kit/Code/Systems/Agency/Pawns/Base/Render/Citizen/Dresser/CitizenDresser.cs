using System;

namespace GameFish;

/// <summary>
/// Puts citizen <see cref="Clothing"/> on a <see cref="PawnCitizenModel"/>.
/// </summary>
[Icon( "checkroom" )]
public partial class CitizenDresser : Component, Component.ExecuteInEditor
{
	protected const int OUTFIT_ORDER = -1000;
	protected const int OUTFIT_DEBUG_ORDER = OUTFIT_ORDER + 50;

	protected bool InGame => this.InGame();
	protected bool InEditor => this.InEditor();

	/// <summary>
	/// Your skin.
	/// </summary>
	[Property]
	[Group( MODEL )]
	[Title( "Component" )]
	[Feature( OUTFIT ), Order( OUTFIT_ORDER )]
	public PawnCitizenModel Model
	{
		get => _model.GetCached( GameObject );
		set => _model = value;
	}

	protected PawnCitizenModel _model;

	/// <summary>
	/// Should the configuration be automatically applied on start/change?
	/// </summary>
	[Property]
	[Title( "Auto-Apply" )]
	[Sync( SyncFlags.FromHost )]
	[Feature( OUTFIT ), Order( OUTFIT_ORDER )]
	public bool AutoApply { get; set; } = true;

	/// <summary>
	/// If true: uses this owner's avatar.
	/// </summary>
	[Property]
	[Sync( SyncFlags.FromHost )]
	[Feature( OUTFIT ), Order( OUTFIT_ORDER )]
	[ToggleGroup( nameof( UseAvatar ), Label = AVATAR )]
	public bool UseAvatar { get; set; } = true;

	/// <summary>
	/// Prevent avatar clothing with these slots from being used.
	/// </summary>
	[Property]
	[Title( "Prevent Slots" )]
	[ToggleGroup( nameof( UseAvatar ) )]
	[Feature( OUTFIT ), Order( OUTFIT_ORDER )]
	public Clothing.Slots PreventAvatarSlots { get; set; } = 0;

	/// <summary>
	/// If true: overlays a default outfit. <br />
	/// Overrides any avatar outfitting.
	/// <br /> <br />
	/// <b> NOTE: </b> Ignores <see cref="PreventAvatarSlots"/>.
	/// </summary>
	[Property]
	[Feature( OUTFIT ), Order( OUTFIT_ORDER )]
	[ToggleGroup( nameof( UseDefault ), Label = DEFAULTS )]
	public bool UseDefault { get; set; }

	[Property, InlineEditor]
	[Title( "Default Outfit" )]
	[ToggleGroup( nameof( UseDefault ) )]
	[Feature( OUTFIT ), Order( OUTFIT_ORDER )]
	public CitizenOutfit DefaultOutfit { get; set; }

	/// <summary>
	/// The currently cached citizen outfit data.
	/// </summary>
	public CitizenOutfit? Avatar { get; protected set; }

	/// <summary>
	/// Allows you to temporarily override the avatar and/or custom outfit. <br />
	/// Useful if you wanted to change clothing or make them a skeleton or something.
	/// </summary>
	[Sync( SyncFlags.FromHost )]
	public CitizenOutfit? Equipped
	{
		get => _equipped;
		set
		{
			_equipped = value;

			if ( AutoApply )
				UpdateOutfit();
		}
	}

	protected CitizenOutfit? _equipped;

	protected ClothingContainer ClothingCache { get; set; }

	public bool HasLoaded { get; protected set; }

	protected override void OnParentChanged( GameObject oldParent, GameObject newParent )
	{
		base.OnParentChanged( oldParent, newParent );

		if ( AutoApply )
			UpdateOutfit();
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( AutoApply && !HasLoaded )
			UpdateOutfit();
	}

	/// <summary>
	/// Allows you to temporarily override the current outfit. <br />
	/// Useful if you wanted to change clothing or make them a skeleton or something.
	/// </summary>
	public virtual void SetEquipped( in CitizenOutfit outfit )
		=> Equipped = outfit;

	/// <summary>
	/// Clears all equipped outfitting.
	/// </summary>
	public virtual void ClearEquipped()
		=> Equipped = default;

	/// <summary>
	/// Sets the current outfit to <paramref name="cnOwner"/>'s avatar.
	/// </summary>
	protected virtual void SetAvatar( Connection cnOwner, bool autoUpdate = true )
	{
		if ( cnOwner is null )
		{
			Avatar = null;
			return;
		}

		// Get the connection's avatar data.
		var avatar = new ClothingContainer();
		avatar.Deserialize( cnOwner.GetUserData( "avatar" ) );

		// Apply it using our custom outfitting.
		var outfit = new CitizenOutfit();

		foreach ( var entry in avatar?.Clothing ?? [] )
		{
			if ( entry?.Clothing.IsValid() ?? false )
			{
				var slotsOver = entry.Clothing.SlotsOver;
				var slotsUnder = entry.Clothing.SlotsUnder;

				if ( PreventAvatarSlots != default )
				{
					if ( slotsOver != default && PreventAvatarSlots.HasFlag( slotsOver ) )
						continue;

					if ( slotsUnder != default && PreventAvatarSlots.HasFlag( slotsUnder ) )
						continue;
				}

				outfit.Add( entry );
			}
		}

		Avatar = outfit;
	}

	protected void AddOutfit( in CitizenOutfit outfit )
	{
		ClothingCache ??= new();

		foreach ( var element in outfit.Clothing ?? [] )
			if ( element.IsValid() )
				ClothingCache.Add( element.GetEntry() );

		ClothingCache.PrefersHuman = outfit.Human ?? ClothingCache.PrefersHuman;
		ClothingCache.Height = outfit.Height ?? ClothingCache.Height;
		ClothingCache.Age = outfit.Age ?? ClothingCache.Age;
	}

	public virtual void UpdateOutfit( Connection cnAvatar = null )
	{
		// Undress..
		ClothingCache ??= new();
		ClothingCache.Clothing?.Clear();

		// User Outfit
		if ( UseAvatar || cnAvatar is not null )
		{
			SetAvatar( cnAvatar ?? Model?.Network?.Owner );

			if ( Avatar.HasValue )
				AddOutfit( Avatar.Value );
		}

		// Default Outfit
		if ( UseDefault )
			AddOutfit( DefaultOutfit );

		// Equipped Outfit
		if ( Equipped.HasValue )
			AddOutfit( Equipped.Value );

		ClothingCache.Normalize();

		try
		{
			if ( !Model.IsValid() || !Model.SkinRenderer.IsValid() )
				return;

			this.Log( "bingus" );

			ClothingCache.Apply( Model.SkinRenderer );
		}
		catch ( Exception e )
		{
			this.Warn( $"{nameof( UpdateOutfit )} exception: {e}" );
		}

		HasLoaded = true;
	}

	/// <summary>
	/// Applies your own avatar to the prefab.
	/// </summary>
	[Button( "Load Local" )]
	[ShowIf( nameof( InEditor ), true )]
	[Feature( OUTFIT ), Group( DEBUG ), Order( OUTFIT_DEBUG_ORDER )]
	public void SetLocalAvatar()
		=> UpdateOutfit( cnAvatar: Connection.Local );

	/// <summary>
	/// Applies the current configuration.
	/// </summary>
	[Button( "Load Current" )]
	[ShowIf( nameof( InEditor ), true )]
	[Feature( OUTFIT ), Group( DEBUG ), Order( OUTFIT_DEBUG_ORDER )]
	public void SetDefaultAvatar()
		=> UpdateOutfit( cnAvatar: null );

	/// <summary>
	/// Broadcasts that the active configuration be re-applied.
	/// </summary>
	[Button( "Force Update" )]
	[ShowIf( nameof( InGame ), true )]
	[Feature( OUTFIT ), Group( DEBUG ), Order( OUTFIT_DEBUG_ORDER )]
	[Rpc.Broadcast( NetFlags.Reliable | NetFlags.HostOnly )]
	public void RpcHostForceUpdate()
		=> UpdateOutfit( cnAvatar: Model?.Network?.Owner );
}
