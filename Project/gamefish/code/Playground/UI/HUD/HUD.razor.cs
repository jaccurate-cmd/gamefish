using System;
using GameFish;

namespace Playground.Razor;

partial class HUD
{
	protected static Pawn Player => Client.Local?.Pawn;

	protected static bool IsAlive => Player?.IsAlive is true;

	protected override int BuildHash()
		=> HashCode.Combine( Player, IsAlive );
}
