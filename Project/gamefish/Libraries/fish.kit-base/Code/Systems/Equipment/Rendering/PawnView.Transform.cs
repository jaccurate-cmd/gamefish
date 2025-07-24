namespace GameFish;

partial class PawnView
{
	/// <summary>
	/// Update this view's transform and sets the main camera's transform too.
	/// </summary>
	protected virtual void UpdateTransform()
	{
		SetModeTransform();

		var cam = Scene?.Camera;

		if ( cam.IsValid() )
			cam.WorldTransform = WorldTransform;
	}

	/// <summary>
	/// Sets this view's transform according to the curent mode.
	/// </summary>
	protected virtual void SetModeTransform()
	{
		var pawn = Pawn;

		if ( !pawn.IsValid() )
			return;

		if ( pawn.GameObject == GameObject )
		{
			this.Warn( this + " was directly on the pawn! It needs to be a child!" );
			Enabled = false;
			return;
		}

		switch ( Mode )
		{
			case Perspective.FirstPerson:
				SetFirstPersonModeTransform();
				break;

			case Perspective.ThirdPerson:
				SetThirdPersonModeTransform();
				break;

			case Perspective.FreeCam:
				SetFreeCamModeTransform();
				break;

			case Perspective.Fixed:
				SetFixedModeTransform();
				break;

			case Perspective.Manual:
				SetManualModeTransform();
				break;

			case Perspective.Custom:
				SetCustomModeTransform();
				break;
		}
	}

	/// <summary>
	/// Sets the transform safely using <see cref="Relative"/> and with transition support.
	/// </summary>
	protected virtual void UpdateRelativeTransform()
	{
		var pawn = Pawn;

		if ( !pawn.IsValid() )
			return;

		if ( Previous is Offset prevOffset )
		{
			// Smoothed transitioning.
			var offsLerped = prevOffset.LerpTo( Relative, TransitionFraction );
			var tLerped = offsLerped.ToWorld( pawn.EyeTransform );

			TrySetPosition( tLerped.Position );
			TrySetRotation( tLerped.Rotation );
		}
		else
		{
			// No transitioning.
			var tRelative = Relative.ToWorld( pawn.EyeTransform );

			TrySetPosition( tRelative.Position );
			TrySetRotation( tRelative.Rotation );
		}
	}

	protected virtual void SetFirstPersonModeTransform()
	{
		UpdateRelativeTransform();
	}

	protected virtual void SetThirdPersonModeTransform()
	{
		UpdateRelativeTransform();
	}

	protected virtual void SetFreeCamModeTransform()
	{
	}

	protected virtual void SetFixedModeTransform()
	{
	}

	protected virtual void SetManualModeTransform()
	{
	}

	protected virtual void SetCustomModeTransform()
	{
	}
}
