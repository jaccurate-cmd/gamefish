using System;
using GameFish;
using Sandbox.UI;

namespace Playground.Razor;

partial class WorldPicker
{
	protected static Editor Editor => Editor.Instance;

	protected static EditorTool ActiveTool => Editor?.Tool;

	protected override void OnClick( MousePanelEvent e )
	{
		base.OnClick( e );

		if ( ActiveTool.IsValid() )
			ActiveTool.OnLeftClick();
	}

	protected override void OnRightClick( MousePanelEvent e )
	{
		base.OnRightClick( e );

		if ( ActiveTool.IsValid() )
			ActiveTool.OnRightClick();
	}

	protected override void OnMiddleClick( MousePanelEvent e )
	{
		base.OnMiddleClick( e );

		if ( ActiveTool.IsValid() )
			ActiveTool.OnMiddleClick();
	}

	protected override int BuildHash()
		=> HashCode.Combine( Editor );
}
