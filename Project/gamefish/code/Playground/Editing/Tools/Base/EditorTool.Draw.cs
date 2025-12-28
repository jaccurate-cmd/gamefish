namespace Playground;

partial class EditorTool
{
	protected virtual void RenderHelpers()
	{
		RenderCursor();
	}

	protected virtual bool TryGetCursorPosition( out Vector3 cursorPos )
	{
		if ( TargetTrace is SceneTraceResult tr && tr.Hit )
		{
			cursorPos = tr.HitPosition;
			return true;
		}

		cursorPos = default;
		return false;
	}

	protected virtual void RenderCursor()
	{
		if ( TryGetCursorPosition( out var cursorPos ) )
			RenderCursor( cursorPos );
	}

	protected virtual void RenderCursor( in Vector3 pos )
	{
		var cSphere = Color.White.WithAlpha( 0.4f );

		this.DrawSphere( 2.5f, pos, Color.Transparent, cSphere, global::Transform.Zero );
	}
}
