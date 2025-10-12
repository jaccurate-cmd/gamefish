using System;

namespace GameFish;

partial class BaseEntity
{
	protected const string NETWORK_INFO =
		"These are this component's intended networking settings.\n\n" +
		"If networking is enabled then you should keep this component on an object(such as a child) separate from other networked components or their settings may conflict!";

	protected bool ShowNetworkProperties => IsNetworkSetupAllowed();

	/// <returns> If specified: the non-negotiable decision to network or not. </returns>
	protected virtual bool? IsNetworkedOverride => null;

	/// <summary>
	/// If true: this entity supports client-side prediction features.
	/// </summary>
	public virtual bool IsPredicted { get; set; }

	/// <summary>
	/// If enabled: network this object <c>OnStart</c> using these settings.
	/// </summary>
	[Property]
	[Title( "Automatic" )]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[ToggleGroup( nameof( NetworkAutomatically ), Label = NETWORKING )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	public virtual bool NetworkAutomatically
	{
		get => IsNetworkedOverride.HasValue ? IsNetworkingForced : _netAuto;
		set => _netAuto = value;
	}

	protected bool _netAuto = false;

	/// <summary>
	/// If true: the component is forcing this to always be networked.
	/// </summary>
	[Property]
	[Title( "Forced (by Component)" )]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[ToggleGroup( nameof( NetworkAutomatically ), Label = NETWORKING )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	public bool IsNetworkingForced => IsNetworkedOverride is true;

	[Property]
	[Title( "Id" )]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[InfoBox( NETWORK_INFO, Tint = EditorTint.Blue, Icon = "wifi" )]
	[ToggleGroup( nameof( NetworkAutomatically ) )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	protected Guid NetworkId => Id;

	/// <summary>
	/// When to network this object(if ever). <br />
	/// Used by the <see cref="UpdateNetworking"/> method.
	/// </summary>
	[Property, ReadOnly]
	[Title( "Network Mode" )]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[ToggleGroup( nameof( NetworkAutomatically ) )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	public NetworkMode NetworkingMode => NetworkingModeDefault;
	protected virtual NetworkMode NetworkingModeDefault => NetworkMode.Object;

	/// <summary>
	/// Who the object can/does belong to. <br />
	/// Used by the <see cref="UpdateNetworking"/> method.
	/// </summary>
	[Property, ReadOnly]
	[Title( "Transfer Mode" )]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[ToggleGroup( nameof( NetworkAutomatically ) )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	public OwnerTransfer NetworkTransferMode => NetworkTransferModeDefault;
	protected virtual OwnerTransfer NetworkTransferModeDefault => OwnerTransfer.Fixed;

	/// <summary>
	/// What to do upon losing a network owner. <br />
	/// Used by the <see cref="UpdateNetworking"/> method.
	/// </summary>
	[Property, ReadOnly]
	[Title( "Orphaned Mode" )]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[ToggleGroup( nameof( NetworkAutomatically ) )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	public NetworkOrphaned NetworkOrphanedMode => NetworkOrphanedModeDefault;
	protected virtual NetworkOrphaned NetworkOrphanedModeDefault => NetworkOrphaned.Destroy;

	/// <summary>
	/// The connection owning this entity.
	/// </summary>
	[Title( "Owner" )]
	[Header( "Debug" )]
	[Property, InlineEditor]
	[Feature( ENTITY ), Order( NETWORK_ORDER )]
	[ToggleGroup( nameof( NetworkAutomatically ) )]
	[ShowIf( nameof( ShowNetworkProperties ), true )]
	public string NetworkOwner => Network?.Owner?.ToString();
	// public Connection NetworkOwner => Network?.Owner;

	/// <summary>
	/// The default owning connection to assign upon network setup.
	/// Will be called <see cref="OnStart"/> if <see cref="NetworkAutomatically"/> is enabled.
	/// </summary>
	/// <returns> The connection to assign in the <see cref="SetupNetworking"/> method. </returns>
	public virtual Connection DefaultNetworkOwner => Network?.Owner ?? Connection.Host;

	/// <returns> If <see cref="SetupNetworking"/> is allowed to execute. </returns>
	protected virtual bool IsNetworkSetupAllowed()
	{
		if ( IsNetworkedOverride.HasValue )
			return IsNetworkedOverride.Value;

		// Always show this stuff in the editor.
		if ( this.InEditor() )
			return true;

		return !IsProxy && Network is not null;
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( NetworkAutomatically && !IsProxy )
			if ( !Network.Active && Network.Owner is null )
				SetupNetworking();
	}

	/// <summary>
	/// By default this networks the object using <see cref="DefaultNetworkOwner"/>. <br />
	/// Called <see cref="OnStart"/> if <see cref="NetworkAutomatically"/> is enabled.
	/// </summary>
	protected virtual void SetupNetworking( bool force = false )
	{
		if ( !IsNetworkSetupAllowed() && !force )
			return;

		// Never clear existing owners set before spawn.
		if ( DefaultNetworkOwner is Connection cn && cn is not null )
			UpdateNetworking( cn );
	}

	/// <summary>
	/// Update this object's networking according to the related properties(by default). <br />
	/// You may use <see cref="DefaultNetworkOwner"/> to respect this component's preference.
	/// </summary>
	/// <param name="cn"> The connection to assign this object to(if any). </param>
	public virtual void UpdateNetworking( Connection cn )
	{
		if ( !GameObject.IsValid() )
			return;

		// this.Log( $"assigning ownership to Connection:[{cn}]" );

		GameObject.NetworkSetup(
			cn: cn,
			orphanMode: NetworkOrphanedMode,
			ownerTransfer: NetworkTransferMode,
			netMode: NetworkingMode,
			ignoreProxy: false
		);
	}
}
