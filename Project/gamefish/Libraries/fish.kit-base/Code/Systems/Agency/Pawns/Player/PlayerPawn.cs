namespace GameFish;

/// <summary>
/// A player-only pawn that uses the <see cref="PlayerController"/>.
/// </summary>
public partial class PlayerPawn : ControllerPawn
{
	public const string PLAYER = "Player";

	public const string MOVEMENT = "Movement";
	public const int MOVEMENT_ORDER = 201;

	public const string DUCKING = "Ducking";
	public const int DUCKING_ORDER = 202;

	public const string JUMPING = "Jumping";
	public const int JUMPING_ORDER = 203;

	/// <summary>
	/// The default walking/flying speed.
	/// </summary>
	[Property]
	[Range( 0, 1000, clamped: false )]
	[Feature( INPUT ), Group( MOVEMENT ), Order( MOVEMENT_ORDER )]
	public virtual float MoveSpeed { get; set; } = 100f;

	[Property]
	[InputAction]
	[Feature( INPUT ), Group( MOVEMENT ), Order( MOVEMENT_ORDER )]
	public string SprintButton { get; set; } = "run";
	public bool HasSprintButton => !string.IsNullOrWhiteSpace( SprintButton );

	[Property]
	[Range( 0, 3, clamped: false )]
	[ShowIf( nameof( HasSprintButton ), true )]
	[Feature( INPUT ), Group( MOVEMENT ), Order( MOVEMENT_ORDER )]
	public virtual float SprintMultiplier { get; set; } = 1.5f;

	[Property]
	[InputAction]
	[Feature( INPUT ), Group( DUCKING ), Order( DUCKING_ORDER )]
	public string DuckButton { get; set; } = "duck";
	public bool HasDuckButton => !string.IsNullOrWhiteSpace( DuckButton );

	[Property]
	[Range( 0, 1000, clamped: false )]
	[ShowIf( nameof( HasDuckButton ), true )]
	[Feature( INPUT ), Group( DUCKING ), Order( DUCKING_ORDER )]
	public virtual float DuckMoveSpeed { get; set; } = 50f;

	[Property]
	[Range( 0, 100, clamped: false )]
	[ShowIf( nameof( HasDuckButton ), true )]
	[Feature( INPUT ), Group( DUCKING ), Order( DUCKING_ORDER )]
	public virtual float DuckLowerSpeed { get; set; } = 10f;

	[Property]
	[Range( 0, 100, clamped: false )]
	[ShowIf( nameof( HasDuckButton ), true )]
	[Feature( INPUT ), Group( DUCKING ), Order( DUCKING_ORDER )]
	public virtual float DuckRaiseSpeed { get; set; } = 15f;

	[Property]
	[InputAction]
	[Feature( INPUT ), Group( JUMPING ), Order( JUMPING_ORDER )]
	public string JumpButton { get; set; } = "jump";
	public bool HasJumpButton => !string.IsNullOrWhiteSpace( JumpButton );

	[Property]
	[Feature( INPUT ), Group( JUMPING ), Order( JUMPING_ORDER )]
	[Range( 0, 1000, clamped: false )]
	[ShowIf( nameof( HasJumpButton ), true )]
	public virtual float JumpSpeed { get; set; } = 250f;

	protected override void OnUpdate()
	{
		base.OnUpdate();

		var allowInput = AllowInput();

		if ( Controller.IsValid() )
			Controller.UseInputControls = allowInput;
	}

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
