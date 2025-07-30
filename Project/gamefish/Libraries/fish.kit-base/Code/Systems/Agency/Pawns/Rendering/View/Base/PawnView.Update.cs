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
			CustomUpdateAction( TargetPawn, this );
		}
		catch ( Exception e )
		{
			this.Warn( e );
		}
	}
}
