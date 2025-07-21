using System;

namespace GameFish;

/// <summary>
/// A basic spectator pawn with three modes to choose from (<see cref="SpectatorModes"/>).
/// </summary>
public partial class SpectatorPawn : ControllerPawn
{
	public const string GROUP_SPECTATOR = "👻 Spectator";

	public enum SpectatorModes
	{
		/// <summary>
		/// Free flight/Noclip mode
		/// </summary>
		[Icon( "flight" )] FreeRoam = 0,
		/// <summary>
		/// First person view from the target's perspective, with optional view model visibility
		/// </summary>
		[Icon( "face" )] FirstPerson,
		/// <summary>
		/// Third person view, centered around the target
		/// </summary>
		[Icon( "videocam" )] ThirdPerson
	}

	/// <summary>
	/// TODO: a hack
	/// </summary>
	private const int SPECTATOR_MODES_LENGTH = (int)SpectatorModes.ThirdPerson + 1;

	[Property]
	[Feature( GROUP_SPECTATOR )]
	public PlayerPawn Target { get; set; }

	[Property]
	[Feature( GROUP_SPECTATOR )]
	public SpectatorModes Mode { get; set; }

	/// <summary>
	/// Requirements for a pawn to be observable.
	///
	/// For example, you can let the players spectate only their teammates.
	/// </summary>
	[Property]
	[Feature( GROUP_SPECTATOR )]
	public Func<BasePlayer, bool> Filter { get; set; }

	private bool IsAnythingEnabled => HasFirstPersonMode || HasThirdPersonMode || HasFreeRoamMode;

	private bool IsModeEnabled( SpectatorModes mode ) => mode switch
	{
		SpectatorModes.FirstPerson => HasFirstPersonMode,
		SpectatorModes.ThirdPerson => HasThirdPersonMode,
		SpectatorModes.FreeRoam => HasFreeRoamMode,
		_ => throw new NotImplementedException( $"Cannot find a flag for mode {mode}" )
	};
	
	private bool InEditor => Scene?.IsEditor ?? true;

	/// <summary>
	/// Choose the next mode available.
	/// </summary>
	[Button( "Next Mode" )]
	[Feature( GROUP_SPECTATOR ), Group( "Debug" )]
	[HideIf(nameof(InEditor), true)]
	public void NextMode()
	{
		if ( !IsAnythingEnabled ) return;

		do
		{
			Mode = (SpectatorModes)(((int)Mode + 1) % SPECTATOR_MODES_LENGTH);
		} while ( !IsModeEnabled( Mode ) );
	}

	/// <summary>
	/// Choose the previous mode available.
	/// </summary>
	[Button( "Previous Mode" )]
	[Feature( GROUP_SPECTATOR ), Group( "Debug" )]
	[HideIf(nameof(InEditor), true)]
	public void PreviousMode()
	{
		if ( !IsAnythingEnabled ) return;

		do
		{
			Mode = (SpectatorModes)((SPECTATOR_MODES_LENGTH + (int)Mode - 1) % SPECTATOR_MODES_LENGTH);
		} while ( !IsModeEnabled( Mode ) );
	}

	/// <summary>
	/// Choose the next mode available.
	/// </summary>
	[Button( "Next Target" )]
	[Feature( GROUP_SPECTATOR ), Group( "Debug" )]
	[HideIf(nameof(InEditor), true)]
	public void NextTarget()
	{
		Log.Error( "TODO:" );
	}

	/// <summary>
	/// Choose the previous mode available.
	/// </summary>
	[Button( "Previous Target" )]
	[Feature( GROUP_SPECTATOR ), Group( "Debug" )]
	[HideIf(nameof(InEditor), true)]
	public void PreviousTarget()
	{
		Log.Error( "TODO:" );
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		switch ( Mode )
		{
			case SpectatorModes.FirstPerson:
				FirstPersonUpdate();
				break;
			
			default:
				throw new NotImplementedException( $"Mode {Mode} is not implemented" );
		}
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();

		switch ( Mode )
		{
			case SpectatorModes.FirstPerson:
			case SpectatorModes.ThirdPerson:
				// rndtrash: There is no need for a FixedUpdate here... not yet
				break;

			default:
				throw new NotImplementedException( $"Mode {Mode} is not implemented" );
		}
	}
}
