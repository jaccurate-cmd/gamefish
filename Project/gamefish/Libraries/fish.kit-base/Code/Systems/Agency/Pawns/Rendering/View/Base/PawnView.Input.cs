namespace GameFish;

partial class PawnView
{
	public const string INPUT = "🕹 Input";

	public const string AIMING = "Aiming";
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

	[Property]
	[Feature( INPUT ), Group( AIMING )]
	public bool PitchClamping { get; set; } = true;

	[Property]
	[Range( 0, 180 )]
	[Feature( INPUT ), Group( AIMING )]
	public FloatRange PitchRange { get; set; } = new( -89.9f, 89.9f );

	/// <summary>
	/// If true: the view will be traced from aiming origin to the destination and collide according to its settings.
	/// </summary>
	[Property]
	[Title( "Enabled" )]
	[Feature( INPUT ), Group( COLLISION )]
	public virtual bool Collision { get; set; } = true;

	/// <summary>
	/// Radius of the sphere collider used when tracing.
	/// </summary>
	[Property]
	[Title( "Radius" )]
	[Range( 1f, 32f, clamped: false )]
	[Feature( INPUT ), Group( COLLISION )]
	public virtual float CollisionRadius { get; set; } = 5f;

	/// <summary>
	/// If true: collide with objects we have explicit ownership over.
	/// </summary>
	[Property]
	[Title( "Owned" )]
	[Range( 1f, 32f, clamped: false )]
	[Feature( INPUT ), Group( COLLISION )]
	public virtual bool CollideOwned { get; set; } = false;

	/// <summary>
	/// Tags of objects that will obstruct the camera view.
	/// </summary>
	[Property]
	[Title( "Hit Tags" )]
	[Feature( INPUT ), Group( COLLISION )]
	public TagSet CollisionTags { get; set; } = ["solid"];

	[Property]
	[Feature( INPUT ), Group( AIMING )]
	public virtual Angles EyeAngles
	{
		get => _eyeAngles;
		set => _eyeAngles = value.WithPitch( value.pitch.Clamp( PitchRange ) );
	}

	protected Angles _eyeAngles;

	public virtual Vector3 EyePosition => Pawn?.EyePosition ?? WorldPosition;

	/// <summary>
	/// An actual rotation. Allows non-Euler fanciness. <br />
	/// A more sophisticated solution would use Rotations instead of Euler angles
	/// but I'm not doing that right now. I'm in a rush and it's not necessary yet.
	/// </summary>
	public virtual Rotation EyeRotation => EyeAngles;
	public Vector3 EyeForward => EyeRotation.Forward;

	public Transform EyeTransform => new( EyePosition, EyeRotation, WorldScale );

	/// <summary>
	/// Distance from this view to the pawn's first-person origin.
	/// </summary>
	public float DistanceFromEye => WorldPosition.Distance( EyePosition );

	/// <summary>
	/// Allows cycling of perspective modes.
	/// </summary>
	protected virtual void HandleInput()
	{
		DoAiming();
		DoModeCycling();
	}

	protected virtual void DoAiming()
	{
		EyeAngles += Input.AnalogLook;
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
