namespace Playground;

partial class EditorTool
{
	protected virtual void RenderHelpers()
	{
		RenderTargetHelpers();
	}

	protected virtual void RenderTargetHelpers()
	{
		if ( TargetTrace is SceneTraceResult tr )
			RenderTargetTrace( in tr );
	}

	protected virtual void RenderTargetTrace( in SceneTraceResult tr )
	{
		RenderCursor( tr.HitPosition );
	}

	protected virtual void RenderCursor( in Vector3 pos )
	{
		var cSphere = Color.White.WithAlpha( 0.4f );

		this.DrawSphere( 3f, pos, Color.Transparent, cSphere, global::Transform.Zero );
	}
}
