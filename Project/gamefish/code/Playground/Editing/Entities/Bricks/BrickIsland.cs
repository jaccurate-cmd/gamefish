namespace Playground;

/// <summary>
/// An entity meant to be bricked up and sire many a brick child.
/// </summary>
public partial class BrickIsland : EditorIsland
{
	/// <summary>
	/// All the bricks within this.
	/// </summary>
	public IEnumerable<BrickBlock> FindBricks()
		=> Components?.GetAll<BrickBlock>( FindMode.Enabled | FindMode.InDescendants ) ?? [];
}
