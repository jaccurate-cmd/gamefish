namespace GameFish;

partial class BaseEntity
{
	public const string NETWORKING = "📶 Networking";
	public const int NETWORK_ORDER = 999999;

	/// <summary>
	/// When to network this object(if ever). <br />
	/// Used by the <see cref="UpdateNetworking"/> method.
	/// </summary>
	[InfoBox( "These are this component's networking defaults when updated from code." )]
	[Property, ReadOnly]
	[Title( "Network Mode" )]
	[Feature( ENTITY ), Group( NETWORKING ), Order( NETWORK_ORDER )]
	public NetworkMode NetworkingMode => NetworkingModeOverride;
	protected virtual NetworkMode NetworkingModeOverride => NetworkMode.Object;

	/// <summary>
	/// Who the object can/does belong to. <br />
	/// Used by the <see cref="UpdateNetworking"/> method.
	/// </summary>
	[Property, ReadOnly]
	[Title( "Transfer Mode" )]
	[Feature( ENTITY ), Group( NETWORKING ), Order( NETWORK_ORDER )]
	public OwnerTransfer NetworkTransferMode => NetworkTransferModeOverride;
	protected virtual OwnerTransfer NetworkTransferModeOverride => OwnerTransfer.Fixed;

	/// <summary>
	/// What to do upon losing a network owner. <br />
	/// Used by the <see cref="UpdateNetworking"/> method.
	/// </summary>
	[Property, ReadOnly]
	[Title( "Orphaned Mode" )]
	[Feature( ENTITY ), Group( NETWORKING ), Order( NETWORK_ORDER )]
	public NetworkOrphaned NetworkOrphanedMode => NetworkOrphanedModeOverride;
	protected virtual NetworkOrphaned NetworkOrphanedModeOverride => NetworkOrphaned.Destroy;

	/// <summary>
	/// Update this object's networking according to the related properties(by default).
	/// </summary>
	/// <param name="cn"> The connection to assign this object to(if any). </param>
	public virtual void UpdateNetworking( Connection cn )
	{
		if ( !Networking.IsHost )
			return;

		GameObject?.NetworkSetup(
			cn: cn,
			orphanMode: NetworkOrphanedMode,
			ownerTransfer: NetworkTransferMode,
			netMode: NetworkingMode,
			ignoreProxy: false
		);
	}
}
