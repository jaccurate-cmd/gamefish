namespace GameFish;

partial class ViewRenderer
{
	protected override bool? IsNetworkedOverride => true;

	protected override NetworkMode NetworkingModeDefault => NetworkMode.Object;
	protected override OwnerTransfer NetworkTransferModeDefault => OwnerTransfer.Fixed;
	protected override NetworkOrphaned NetworkOrphanedModeDefault => NetworkOrphaned.ClearOwner;

	public override Connection DefaultNetworkOwner => View?.Network?.Owner;
}
