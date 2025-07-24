using System;

namespace GameFish;

partial class PawnView
{
	protected virtual void OnPerspectiveUpdate( in float deltaTime )
	{
		switch ( _mode )
		{
			case Perspective.FirstPerson:
				OnFirstPersonModeUpdate( in deltaTime );
				break;

			case Perspective.ThirdPerson:
				OnThirdPersonModeUpdate( in deltaTime );
				break;

			case Perspective.FreeCam:
				OnFreeCamModeUpdate( in deltaTime );
				break;

			case Perspective.Fixed:
				OnFixedModeUpdate( in deltaTime );
				break;

			case Perspective.Manual:
				OnManualModeUpdate( in deltaTime );
				break;

			case Perspective.Custom:
				OnCustomModeUpdate( in deltaTime );
				break;
		}
	}

	protected virtual void OnFirstPersonModeUpdate( in float deltaTime )
	{
		var pawn = Pawn;

		if ( !pawn.IsValid() )
			return;

		Relative = new();

		UpdateViewModel( in deltaTime );
	}

	protected virtual void OnThirdPersonModeUpdate( in float deltaTime )
	{
		var pawn = Pawn;

		if ( !pawn.IsValid() )
			return;

		var tPos = Vector3.Backward * 150f;

		Relative = new( tPos, Rotation.Identity );
	}

	protected virtual void OnFreeCamModeUpdate( in float deltaTime )
	{
		this.Warn( $"Mode {Perspective.FreeCam} is not yet implemented." );
	}

	protected virtual void OnFixedModeUpdate( in float deltaTime )
	{
	}

	protected virtual void OnManualModeUpdate( in float deltaTime )
	{
	}

	protected virtual void OnCustomModeUpdate( in float deltaTime )
	{
		try
		{
			CustomUpdateAction( Pawn, this );
		}
		catch ( Exception e )
		{
			this.Warn( e );
		}
	}
}
