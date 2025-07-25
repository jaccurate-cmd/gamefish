namespace GameFish;

partial class PawnView
{
	public const string INPUT = "🕹 Input";

	[Property]
	[Range( 0, 180 )]
	[Feature( INPUT )]
	[Title( "Pitch Clamp" )]
	public float PitchClamp { get; set; } = 90f;

	/// <summary>
	/// If true: you can press buttons to cycle through perspectives.
	/// </summary>
	[Property]
	[Title( "Allow" )]
	[Feature( INPUT ), Group( CYCLING )]
	public virtual bool AllowCyclingMode { get; set; } = false;

	/// <summary>
	/// The button to press to select the next mode.
	/// </summary>
	[Property]
	[InputAction]
	[Title( "Forward" )]
	[Feature( INPUT ), Group( CYCLING )]
	[ShowIf( nameof( AllowCyclingMode ), true )]
	public string CycleModeForwardAction { get; set; } = "View";

	/// <summary>
	/// The button to press to select the next mode.
	/// </summary>
	[Property]
	[InputAction]
	[Title( "Backward" )]
	[Feature( INPUT ), Group( CYCLING )]
	[ShowIf( nameof( AllowCyclingMode ), true )]
	public string CycleModeBackwardAction { get; set; }

	/// <summary>
	/// Allows cycling of perspective modes.
	/// </summary>
	protected virtual void HandleInput()
	{
		if ( !AllowCyclingMode )
			return;

		if ( !string.IsNullOrEmpty( CycleModeForwardAction ) )
			if ( Input.Pressed( CycleModeForwardAction ) )
				CycleMode( 1 );

		if ( !string.IsNullOrEmpty( CycleModeBackwardAction ) )
			if ( Input.Pressed( CycleModeBackwardAction ) )
				CycleMode( -1 );
	}
}
