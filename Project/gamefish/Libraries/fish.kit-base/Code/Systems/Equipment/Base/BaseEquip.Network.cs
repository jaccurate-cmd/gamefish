
namespace GameFish;

partial class BaseEquip
{
	protected override NetworkOrphaned NetworkOrphanedModeOverride => NetworkOrphaned.ClearOwner;
}
