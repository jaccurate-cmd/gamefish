namespace Playground;

public partial class BrickTool : ShapeTool
{
	public const int BRICK_SIZE_MIN = 16;
	public const int BRICK_SIZE_MAX = 32;
	public const float BRICK_SIZE_STEP = 8;
	public const float BRICK_MODEL_SIZE = 16;

	[Property]
	[Title( "Prefab Size" )]
	[Range( 1f, 32f, clamped: false )]
	[Feature( EDITOR ), Group( PREFABS ), Order( PREFABS_ORDER - 1 )]
	public float BrickPrefabSize
	{
		get => _brickPrefabSize.Max( 1f );
		protected set => _brickPrefabSize = value.Max( 1f );
	}

	protected float _brickPrefabSize = 16f;

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

		const FindMode findMode = FindMode.EnabledInSelf | FindMode.InDescendants;

		if ( TargetObject.IsValid() )
			if ( TargetObject.Components.TryGet<BrickBlock>( out _, findMode ) )
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
		tShape.Scale /= BrickPrefabSize;

		if ( Editor.TryFindIsland( OriginObject, out var island ) )
			if ( TrySpawnObject( ShapePrefab, tShape, island, out _ ) )
				return true;

		if ( TrySpawnObject( ShapePrefab, tShape, out _, withIsland: true ) )
			return true;

		return false;
	}

	protected override void OnObjectSpawned( EditorObject e, EditorIsland parent )
	{
		if ( e.IsValid() && e is BrickBlock brick )
			brick.RandomizeColor();

		base.OnObjectSpawned( e, parent );
	}
}
