using System.Text.Json.Serialization;

namespace Playground;

/// <summary>
/// Makes shapes. Previews said shape.
/// </summary>
public abstract class ShapeTool : EditorTool
{
	[Property]
	[Title( "Shape" )]
	[Feature( EDITOR ), Group( PREFABS ), Order( PREFABS_ORDER )]
	public PrefabFile ShapePrefab { get; set; }

	[Property]
	[ToolSetting]
	[Range( 0f, 100f )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public virtual float ScrollSensitivity { get; set; } = 32f;

	[Property]
	[ToolSetting]
	[Range( 0f, 4096f )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public float Distance { get; set; } = 512f;

	[Property]
	[Range( 0f, 4096f )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public FloatRange DistanceRange { get; set; } = new( 16f, 2048f );

	/// <summary>
	/// The coordinates of the shape(in local or world space).
	/// </summary>
	[Property, JsonIgnore, WideMode]
	[ShowIf( nameof( InGame ), true )]
	[Feature( EDITOR ), Group( DEBUG ), Order( EDITOR_DEBUG_ORDER )]
	public List<(Vector3 Position, Rotation Rotation)> Points { get; set; }

	/// <summary>
	/// Are our <see cref="Points"/> not null with at least one point defined?
	/// </summary>
	public bool HasPoints => Points?.Count > 0;

	/// <summary>
	/// Do we have enough(or too many) <see cref="Points"/>?
	/// </summary>
	public bool AtLimit => HasPoints && Points.Count >= PointLimit;

	/// <summary>
	/// Can something be made of this shape?
	/// </summary>
	public virtual bool ValidShape => Points?.Count >= 2;

	/// <summary>
	/// The maximum number of points for making this shape.
	/// </summary>
	public abstract int PointLimit { get; }

	public override void OnExit()
	{
		base.OnExit();

		Clear();
	}

	protected override void Clear()
	{
		base.Clear();

		Points?.Clear();
	}

	public override bool TryRightClick()
	{
		if ( HasPoints )
		{
			Clear();
			return true;
		}

		return false;
	}

	protected override void OnPrimary( in SceneTraceResult tr )
	{
		if ( TryGetCursorPosition( out var cursorPos ) )
			TryAddPoint( cursorPos, Rotation.LookAt( tr.Normal ) );
	}

	public override bool TryMouseWheel( in Vector2 dir )
	{
		var scroll = dir.y != 0f ? -dir.y : dir.x;
		scroll *= ScrollSensitivity;

		OnScroll( in scroll );

		return true;
	}

	protected virtual void OnScroll( in float scroll )
		=> Distance = (Distance + scroll).Clamp( DistanceRange );

	protected override void OnReload( in SceneTraceResult tr )
	{
		base.OnReload( tr );

		Clear();
	}

	protected override void RenderHelpers()
	{
		base.RenderHelpers();

		RenderShape();
	}

	protected virtual void RenderShape()
	{
		RenderBoxShape();
	}

	protected virtual void RenderBoxShape()
	{
		if ( !HasPoints )
			return;

		var tOrigin = GetShapeOrigin().WithScale( 1f );

		var points = Points?.Select( pr => pr.Position ).ToList();

		if ( TryGetCursorPosition( out var cursorPos ) )
			points.Add( tOrigin.PointToLocal( cursorPos ) );

		var box = BBox.FromPoints( points );
		box = BBox.FromPositionAndSize( box.Center, box.Size - 0.1f );
		// .Grow( -tOrigin.PointToLocal( float.Epsilon ).Length );

		this.DrawBox( box, ColorOutline, ColorFilled, tOrigin );
	}

	protected virtual bool TryAddPoint( Vector3 pos, Rotation r )
	{
		if ( !IsClientAllowed( Client.Local ) )
			return false;

		if ( !HasPoints && TargetObject.IsValid() )
		{
			var tTarget = TargetObject.WorldTransform;
			var offset = tTarget.ToLocal( new( pos, r ) );

			TrySetOrigin( TargetObject, TargetComponent, offset );
		}

		var tOrigin = GetShapeOrigin();

		pos = tOrigin.PointToLocal( pos );
		r = tOrigin.RotationToLocal( r );

		Points ??= [];

		if ( AtLimit )
			Points.RemoveAt( Points.Count - 1 );

		if ( !AtLimit )
			Points.Add( (pos, r) );

		OnPointAdded( pos, r );

		return true;
	}

	protected virtual void OnPointAdded( in Vector3 pos, in Rotation r )
	{
		if ( !AtLimit )
			return;

		if ( TryCreateShape( out _ ) )
			Clear();
	}

	public virtual Transform GetShapeOrigin( in Transform? tOverride = null )
	{
		// Might be snapping to something.
		if ( !OriginObject.IsValid() )
			return tOverride ?? global::Transform.Zero;

		var tOrigin = tOverride ?? OriginObject.WorldTransform;

		if ( OriginOffset is Offset offset )
			tOrigin = tOrigin.WithOffset( offset );

		return tOrigin;
	}

	protected abstract bool TryCreateShape( out GameObject obj );
}
