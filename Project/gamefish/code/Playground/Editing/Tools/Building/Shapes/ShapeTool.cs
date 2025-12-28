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
	[Title( "Shape Size" )]
	[Feature( EDITOR ), Group( PREFABS ), Order( PREFABS_ORDER )]
	public Vector3 ShapeSize { get; set; } = 50f;

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
	[ToolSetting]
	[Range( 0f, 4096f )]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public FloatRange DistanceRange { get; set; } = new( 16f, 2048f );

	/// <summary>
	/// The coordinates of the shape(in local or world space).
	/// </summary>
	[Property, JsonIgnore]
	[Feature( EDITOR ), Group( DEBUG ), Order( EDITOR_DEBUG_ORDER )]
	public List<(Vector3 Position, Rotation Rotation)> Points { get; set; }

	/// <summary>
	/// Are our <see cref="Points"/> not null with at least one point defined?
	/// </summary>
	public bool HasPoints => Points?.Count > 0;

	/// <summary>
	/// Can something be made of this shape?
	/// </summary>
	public virtual bool ValidShape => Points?.Count >= 2;

	/// <summary>
	/// Do we have enough(or too many) <see cref="Points"/>?
	/// </summary>
	public bool AtLimit => HasPoints && Points.Count >= PointLimit;

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
		if ( TargetEntity.IsValid() || HasPoints )
		{
			Clear();
			return true;
		}

		return false;
	}

	protected override void OnPrimary( in SceneTraceResult tr )
	{
		TryAddPoint( tr.EndPosition, Rotation.LookAt( tr.Direction ) );
	}

	protected override void OnScroll( in Vector2 scroll )
	{
		var yScroll = scroll.y != 0f ? -scroll.y : scroll.x;

		Distance = (Distance + yScroll).Clamp( DistanceRange );
	}

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
		if ( !HasPoints )
			return;

		var points = Points?.Select( pr => pr.Position );
		var box = BBox.FromPoints( points ).Grow( -0.01f );

		this.DrawBox( box, ColorOutline, ColorFilled, global::Transform.Zero );
	}

	protected virtual bool TryAddPoint( Vector3 pos, Rotation r )
	{
		if ( !IsClientAllowed( Client.Local ) )
			return false;

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

	protected abstract bool TryCreateShape( out GameObject obj );
}
