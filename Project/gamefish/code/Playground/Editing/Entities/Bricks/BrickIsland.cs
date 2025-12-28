namespace Playground;

/// <summary>
/// An entity meant to be bricked up and sire many a brick child.
/// </summary>
public partial class BrickIsland : EditorObjectGroup
{
	/// <summary>
	/// All the bricks in this island.
	/// </summary>
	public IEnumerable<BrickBlock> Bricks => Components?.GetAll<BrickBlock>() ?? [];
}
