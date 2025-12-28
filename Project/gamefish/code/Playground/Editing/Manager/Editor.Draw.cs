namespace Playground;

partial class Editor
{
	/// <summary>
	/// How close must things be to display their helpers?
	/// </summary>
	[Property]
	[Title( "Helper Radius" )]
	[Feature( EDITOR ), Group( SETTINGS )]
	public float DrawNearbyHelperRadius { get; set; } = 2048f;

	protected void DrawNearbyHelpers()
	{
		if ( !IsOpen )
			return;

		if ( !Scene.IsValid() || !Scene.Camera.IsValid() )
			return;

		var origin = Scene.Camera.WorldPosition;
		var radius = DrawNearbyHelperRadius;

		if ( !ITransform.IsValid( origin ) || !ITransform.IsValid( radius ) )
			return;

		foreach ( var ent in Scene.GetAll<EditorObject>() )
			if ( origin.Distance( ent.Center ) < radius )
				ent.RenderHelpers();
	}
}
