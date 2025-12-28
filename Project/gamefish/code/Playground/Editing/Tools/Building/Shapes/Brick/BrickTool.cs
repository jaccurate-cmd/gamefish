namespace Playground;

public partial class BlockTool : BrickTool;

public partial class BrickTool : ShapeTool
{
	public const int BRICK_POWER_MIN = 3;
	public const int BRICK_POWER_MAX = 8;
	public const int BRICK_POWER_DEFAULT = 5;

	/// <summary>
	/// Scales the size of the brick by this power.
	/// <b> TODO: </b> Your mother.
	/// </summary>
	[Property]
	[ToolSetting]
	[Range( BRICK_POWER_MIN, BRICK_POWER_MIN )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public int BrickPower
	{
		get => _brickPower.Clamp( BRICK_POWER_MIN, BRICK_POWER_MIN );
		set => _brickPower = value.Clamp( BRICK_POWER_MIN, BRICK_POWER_MIN );
	}

	protected int _brickPower = 4;

	/// <summary>
	/// Is this being attached to an island?
	/// </summary>
	public bool HasParentIsland { get; protected set; }
	public BrickIsland ParentIsland { get; protected set; }
	public Transform? LastIslandTransform { get; protected set; }

	public override int PointLimit => 2;

	protected override void OnScroll( in Vector2 scroll )
	{
		if ( HoldingShift )
		{
			_brickPower += scroll.y.Round().FloorToInt();
			return;
		}

		base.OnScroll( scroll );
	}

	protected override void OnPointAdded( in Vector3 pos, in Rotation r )
	{
		base.OnPointAdded( pos, r );
	}

	protected override bool TryCreateShape( out GameObject obj )
	{
		obj = null;

		if ( !HasPoints || !ValidShape )
			return false;

		var tShape = GetShapeOrigin();

		if ( !TrySpawnPrefab( ShapePrefab, out obj, tShape ) )
			return false;

		return true;
	}

	protected override void OnPrefabSpawned( GameObject obj )
	{
		if ( !obj.IsValid() )
			return;

		const FindMode findMode = FindMode.Enabled | FindMode.InDescendants;

		foreach ( var brick in obj.Components.GetAll<BrickBlock>( findMode ).ToArray() )
			brick.RandomizeColor();

		base.OnPrefabSpawned( obj );
	}
}
