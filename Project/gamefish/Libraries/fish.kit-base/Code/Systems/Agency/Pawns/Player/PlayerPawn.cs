namespace GameFish;

/// <summary>
/// A player-only pawn that uses the <see cref="PlayerController"/>.
/// </summary>
public partial class PlayerPawn : ControllerPawn
{
	public const string PLAYER = "Player";

	[Property]
	[Range( 0, 180 )]
	[Feature( INPUT )]
	[Title( "Pitch Clamp" )]
	public float FreeRoamPitchClamp { get; set; } = 90f;

	[Property]
	[Feature( INPUT )]
	[Range( 1, 1000, clamped: false )]
	public float Speed { get; set; } = 50f;

	[Property]
	[InputAction]
	[Feature( INPUT )]
	public string SprintButton { get; set; } = "run";
	public bool HasSprintButton => !string.IsNullOrWhiteSpace( SprintButton );

	[Property]
	[Feature( INPUT )]
	[Range( 1, 1000, clamped: false )]
	[ShowIf( nameof( HasSprintButton ), null )]
	public float SprintSpeed { get; set; } = 100f;

	[Property]
	[InputAction]
	[Feature( INPUT )]
	public string JumpButton { get; set; } = "jump";

	[Property]
	[InputAction]
	[Feature( INPUT )]
	public string DuckButton { get; set; } = "duck";

	/// <summary>
	/// Only player <see cref="Agent"/>s can own a player pawn.
	/// </summary>
	public override bool AllowOwnership( Agent agent )
	{
		if ( !agent.IsValid() || !agent.IsPlayer )
			return false;

		if ( !base.AllowOwnership( agent ) )
			return false;

		return true;
	}
}
