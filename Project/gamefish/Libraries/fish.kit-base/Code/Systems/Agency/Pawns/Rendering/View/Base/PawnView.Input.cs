namespace GameFish;

partial class PawnView
{
	public const string COLLISION = "Collision";

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
	public virtual string CycleModeForwardAction { get; set; } = "View";

	/// <summary>
	/// The button to press to select the next mode.
	/// </summary>
	[Property]
	[InputAction]
	[Title( "Backward" )]
	[Feature( INPUT ), Group( CYCLING )]
	[ShowIf( nameof( AllowCyclingMode ), true )]
	public virtual string CycleModeBackwardAction { get; set; }

	/// <summary>
	/// Allows cycling of perspective modes.
	/// </summary>
	protected virtual void HandleInput()
	{
		DoModeCycling();
	}

	protected virtual void DoModeCycling()
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
