namespace Playground;

public partial class BlockTool : BrickTool;

public partial class BrickTool : ShapeTool
{
	public const int BRICK_SIZE_MIN = 16;
	public const int BRICK_SIZE_MAX = 32;
	public const float BRICK_SIZE_STEP = 16;

	/// <summary>
	/// Scales the size of the brick by this power.
	/// <b> TODO: </b> Your mother.
	/// </summary>
	[Property]
	[ToolSetting]
	[Range( BRICK_SIZE_MIN, BRICK_SIZE_MAX )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public int BrickSize
	{
		get => _brickSize.Clamp( BRICK_SIZE_MIN, BRICK_SIZE_MAX );
		set
		{
			var step = (value / BRICK_SIZE_STEP).Round();
			var size = (step * BRICK_SIZE_STEP).CeilToInt();
			_brickSize = size.Clamp( BRICK_SIZE_MIN, BRICK_SIZE_MAX );
		}
	}

	protected int _brickSize = 16;

	/// <summary>
	/// Is this being attached to an island?
	/// </summary>
	public bool HasParentIsland { get; protected set; }
	public BrickIsland ParentIsland { get; protected set; }
	public Transform? LastIslandTransform { get; protected set; }

	public override int PointLimit => 2;

	public Vector3 SnapToBrick( Transform tBrick, Vector3 pos )
	{
		tBrick.Scale = 1f;

		pos = tBrick.PointToLocal( pos );

		pos.x = (pos.x / BrickSize).Round() * BrickSize;
		pos.y = (pos.y / BrickSize).Round() * BrickSize;
		pos.z = (pos.z / BrickSize).Round() * BrickSize;

		pos = tBrick.PointToWorld( pos );

		return pos;
	}

	protected override void OnPrimary( in SceneTraceResult tr )
	{
		if ( !TryGetCursorPosition( out var cursorPos ) )
			return;

		if ( TargetObject.IsValid() )
		{
			var rTarget = TargetObject.WorldRotation;
			// var vAxis = rTarget.ClosestAxis( Vector3.Forward );
			var rAligned = Rotation.LookAt( tr.Normal, rTarget.Forward );

			TryAddPoint( cursorPos, rAligned );
			return;
		}

		TryAddPoint( cursorPos, Rotation.LookAt( Vector3.Forward, tr.Normal ) );
	}

	protected override bool TryGetCursorPosition( out Vector3 cursorPos )
	{
		if ( TargetTrace is not SceneTraceResult tr || !tr.Hit )
		{
			cursorPos = default;
			return false;
		}

		var dist = tr.Distance.Min( Distance );
		cursorPos = tr.StartPosition + (tr.Direction * dist);

		if ( OriginObject.IsValid() )
		{
			cursorPos = SnapToBrick( GetShapeOrigin( OriginObject.WorldTransform ), cursorPos );
			return true;
		}

		bool isBrick = false;

		const FindMode findMode = FindMode.EnabledInSelf | FindMode.InAncestors;

		if ( TargetObject.IsValid() )
			if ( TargetObject.Components.TryGet<BrickIsland>( out _, findMode ) )
				isBrick = true;

		if ( isBrick )
		{
			var tBrick = GetShapeOrigin( TargetObject.WorldTransform );
			cursorPos = SnapToBrick( tBrick, cursorPos );
		}

		return true;
	}

	protected override void OnScroll( in float scroll )
	{
		if ( HoldingShift )
		{
			BrickSize += scroll.Round().CeilToInt();
			return;
		}

		base.OnScroll( scroll );
	}

	protected override bool TryCreateShape( out GameObject obj )
	{
		obj = null;

		if ( !HasPoints || !ValidShape )
			return false;

		var points = Points?.Select( pr => pr.Position ).ToList();
		var box = BBox.FromPoints( points );

		var tOrigin = GetShapeOrigin();
		var tShape = tOrigin.ToWorld( new( box.Mins, Rotation.Identity, box.Size ) );

		tShape.Scale = tShape.Scale.ComponentMax( BRICK_SIZE_MIN );

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
