using System;
using GameFish;

namespace Playground.Razor;

partial class EditorMenu
{
	protected static Editor Editor => Editor.Instance;

	public static bool IsOpen => Editor.TryGetInstance( out var e ) && e.IsOpen;

	protected override int BuildHash()
		=> HashCode.Combine( IsOpen );
}
