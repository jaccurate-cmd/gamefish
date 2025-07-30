namespace GameFish;

/// <summary>
/// A <see cref="PawnView"/> that can look through a <see cref="global::GameFish.SpectatorPawn"/> target's eyes.
/// </summary>
public partial class SpectatorView : PawnView
{
	public SpectatorPawn SpectatorPawn => ParentPawn as SpectatorPawn;

	public override BasePawn TargetPawn => SpectatorPawn is SpectatorPawn spec && spec.IsValid()
		? spec.Spectating.IsValid() ? spec.Spectating : spec
		: null;

	public override void FrameSimulate( in float deltaTime )
	{
		base.FrameSimulate( deltaTime );

		EnsureFlyingMode();
	}

	public override void CycleMode( in int dir )
	{
		if ( !EnsureFlyingMode() )
			base.CycleMode( dir );
	}

	/// <summary>
	/// Makes sure we're in first person while not spectating someone.
	/// </summary>
	/// <returns> If the mode was forced to first person. </returns>
	protected bool EnsureFlyingMode()
	{
		var spec = SpectatorPawn;

		// Ensure first person while not spectating someone.
		if ( spec.IsValid() && !spec.Spectating.IsValid() )
		{
			if ( Mode != Perspective.FirstPerson )
			{
				Mode = Perspective.FirstPerson;
				return true;
			}
		}

		return false;
	}

	public override Transform GetOrigin()
	{
		if ( SpectatorPawn is SpectatorPawn spec && !spec.Spectating.IsValid() )
			return spec.EyeTransform.WithRotation( EyeRotation );

		var targetPawn = TargetPawn;

		if ( !targetPawn.IsValid() )
			return EyeTransform;

		return Mode is not Perspective.FirstPerson
			? targetPawn.EyeTransform.WithRotation( EyeRotation )
			: targetPawn.EyeTransform;
	}

	protected override void DoAiming()
	{
		/*
		// Prevent aiming while spectating in first person.
		if ( Mode is Perspective.FirstPerson )
			if ( SpectatorPawn is SpectatorPawn spec && spec.IsValid() )
				if ( spec.Spectating.IsValid() )
					return;
		*/

		base.DoAiming();
	}

	protected override void OnFirstPersonModeUpdate( in float deltaTime )
	{
		base.OnFirstPersonModeUpdate( deltaTime );

		if ( SpectatorPawn is SpectatorPawn spec && spec.Spectating.IsValid() )
			EyeRotation = spec.Spectating.EyeRotation;
	}
}
