namespace Playground;

partial class EditorTool
{
	protected override bool? IsNetworkedOverride => true;
	protected override bool IsNetworkedAutomatically => true;

	protected override NetworkMode NetworkingModeDefault => NetworkMode.Object;
	protected override OwnerTransfer NetworkTransferModeDefault => OwnerTransfer.Fixed;
	protected override NetworkOrphaned NetworkOrphanedModeDefault => NetworkOrphaned.Host;

	public override Connection DefaultNetworkOwner => Connection.Host;

	/// <summary>
	/// Spawns a prefab safely and with auto-configuration.
	/// </summary>
	public virtual bool TrySpawnPrefab( PrefabFile prefab, out GameObject obj, Transform? tWorld = null, in bool autoNetwork = true )
	{
		if ( !IsClientAllowed( Client.Local ) )
		{
			obj = null;
			return false;
		}

		var bSpawned = tWorld.HasValue
			? prefab.TrySpawn( tWorld ?? global::Transform.Zero, out obj )
			: prefab.TrySpawn( out obj );

		if ( !bSpawned || !obj.IsValid() )
			return false;

		if ( autoNetwork )
			OnPrefabSpawned( obj );

		return true;
	}

	protected virtual void OnPrefabSpawned( GameObject obj )
	{
		if ( !obj.IsValid() )
			return;

		obj.NetworkSetup(
			cn: Connection.Host,
			orphanMode: NetworkOrphaned.ClearOwner,
			ownerTransfer: OwnerTransfer.Takeover,
			netMode: NetworkMode.Object,
			ignoreProxy: true
		);
	}
}
