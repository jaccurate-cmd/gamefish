namespace GameFish;

/// <summary>
/// A <see cref="PawnView"/> that can look through a <see cref="SpectatorPawn"/> target's eyes.
/// </summary>
public partial class SpectatorView : PawnView
{
	public SpectatorPawn Spectator => ModuleParent as SpectatorPawn;

	public override BasePawn Pawn => Spectator is SpectatorPawn spec && spec.IsValid()
		? spec.Spectating ?? base.Pawn
		: base.Pawn;

	public override void FrameSimulate( in float deltaTime )
	{
		base.FrameSimulate( deltaTime );

		var spec = Spectator;

		// Ensure first person while not spectating someone.
		if ( spec.IsValid() && !spec.Spectating.IsValid() )
			if ( Mode != Perspective.FirstPerson )
				Mode = Perspective.FirstPerson;
	}

	public override void CycleMode( in int dir )
	{
		var spec = Spectator;

		if ( spec.IsValid() && !spec.Spectating.IsValid() )
		{
			Mode = Perspective.FirstPerson;
			return;
		}

		base.CycleMode( dir );
	}
}
