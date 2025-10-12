namespace GameFish;

partial class BaseEquip : Component.INetworkSpawn
{
	protected override bool? IsNetworkedOverride => true;

	protected override NetworkMode NetworkingModeDefault => NetworkMode.Object;
	protected override OwnerTransfer NetworkTransferModeDefault => OwnerTransfer.Fixed;
	protected override NetworkOrphaned NetworkOrphanedModeDefault => NetworkOrphaned.ClearOwner;

	public override Connection DefaultNetworkOwner => Owner?.Network.Owner;

	[Property]
	[Title( "Owner" )]
	[Feature( EQUIP ), Group( DEBUG )]
	protected BasePawn DebugOwner => Owner;

	[Sync( SyncFlags.FromHost )]
	public BasePawn Owner
	{
		get => _owner;
		protected set
		{
			_owner = value;
			OnOwnerSet( _owner );
		}
	}

	protected BasePawn _owner;

	public override void UpdateNetworking( Connection cn )
	{
		if ( !GameObject.IsValid() )
			return;

		GameObject.NetworkSetup(
			cn: cn,
			orphanMode: NetworkOrphanedMode,
			ownerTransfer: NetworkTransferMode,
			netMode: NetworkingMode,
			ignoreProxy: Networking.IsHost
		);
	}

	public void OnNetworkSpawn( Connection owner )
	{
		SetOwner( Server.FindPawn( owner ) );
	}

	/// <summary>
	/// Allows the host to set assign the owner of the equipment.
	/// </summary>
	public void SetOwner( BasePawn owner )
	{
		if ( !Networking.IsHost )
			return;

		if ( !GameObject.IsValid() || this.InEditor() )
			return;

		var prevOwner = Network.Owner;

		if ( !owner.IsValid() && prevOwner is not null && prevOwner != owner.Network.Owner )
			Network.DropOwnership();

		var cn = owner?.Network?.Owner;

		if ( !owner.IsValid() || cn is null )
		{
			// TODO: ditch that shit on the ground
			Network.DropOwnership();

			GameObject.SetParent( null );
		}
		else
		{
			// Can't change the parent if it's owned by a client.
			if ( Network.Owner is not null && cn != Network.Owner )
				Network.DropOwnership();

			// Keep that thang on 'em.. or not.
			GameObject.SetParent( owner.GameObject );

			// Force the owner to be a parent.
			Network.AssignOwnership( cn );
		}

		Owner = owner;
	}

	protected void OnOwnerSet( BasePawn owner )
	{
		if ( !this.InGame() || !GameObject.IsValid() )
			return;

		if ( !owner.IsValid() )
			return;

		var inv = owner.GetModule<PawnEquipment>();

		if ( IsProxy )
			goto OnEquip;

		if ( !inv.IsValid() )
		{
			EquipState = EquipState.Dropped;
		}
		else if ( inv.ActiveEquip.IsValid() )
		{
			// Always holstered if not the active equip.
			if ( inv.ActiveEquip != this )
				EquipState = EquipState.Holstered;
		}
		else
		{
			// This deploys the equipment.
			inv.ActiveEquip = this;
		}

		// Call OnEquip on all clients.
		OnEquip:

		if ( owner.IsValid() )
			OnEquip( owner );

		if ( Networking.IsHost && inv.IsValid() )
			inv.RefreshList();
	}
}
