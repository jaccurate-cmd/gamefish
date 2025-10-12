namespace GameFish;

partial class BaseController
{
	public const string AIMING = "Aiming";
	public const int AIMING_ORDER = 1000;

	public const string EYEPOS = "Eye Position";
	public const int EYEPOS_ORDER = 2000;

	public const string MOVEMENT = "Movement";
	public const int MOVEMENT_ORDER = 3000;

	public const string SPRINTING = "Sprinting";
	public const int SPRINTING_ORDER = 4000;

	public const string DUCKING = "Ducking";
	public const int DUCKING_ORDER = 5000;

	public const string JUMPING = "Jumping";
	public const int JUMPING_ORDER = 6000;

	/// <summary>
	/// Should the owner be able to input their movement?
	/// </summary>
	[Property]
	[Feature( INPUT ), Order( MOVEMENT_ORDER )]
	[ToggleGroup( nameof( AllowMovement ), Label = MOVEMENT )]
	public virtual bool AllowMovement { get; set; } = true;

	/// <summary>
	/// The default walking/flying speed.
	/// </summary>
	[Property]
	[Title( "Move Speed (Default)" )]
	[Range( 0f, 1000f, clamped: false )]
	[ToggleGroup( nameof( AllowMovement ) )]
	[Feature( INPUT ), Order( MOVEMENT_ORDER )]
	public virtual float MoveSpeed { get; set; } = 250f;

	/// <summary>
	/// Should the owner be able to increase their speed?
	/// </summary>
	[Property]
	[Feature( INPUT ), Order( SPRINTING_ORDER )]
	[ToggleGroup( nameof( AllowSprinting ), Label = SPRINTING )]
	public virtual bool AllowSprinting { get; set; } = true;

	/// <summary>
	/// If true: sprinting is on when not held and is toggled off instead.
	/// </summary>
	[Property]
	[Title( "Starts Enabled" )]
	[Feature( INPUT ), Order( SPRINTING_ORDER )]
	[ToggleGroup( nameof( AllowSprinting ), Label = SPRINTING )]
	public virtual bool SprintDefault { get; set; } = false;

	/// <summary>
	/// The button to run. <br />
	/// Set this to blank/null to disable it.
	/// </summary>
	[Property]
	[InputAction]
	[ToggleGroup( nameof( AllowSprinting ) )]
	[Feature( INPUT ), Order( SPRINTING_ORDER )]
	public virtual string SprintButton { get; set; } = "run";
	public virtual bool HasSprintButton => !string.IsNullOrWhiteSpace( SprintButton );

	public virtual bool ShouldSprint => AllowSprinting && HasSprintButton
		&& (Input.Down( SprintButton ) == !SprintDefault);

	[Sync]
	public bool IsSprinting { get; set; }

	public virtual float GetSprintSpeed( in float? baseSpeed = null )
		=> (baseSpeed ?? MoveSpeed) * SprintMultiplier;

	[Property]
	[ToggleGroup( nameof( AllowSprinting ) )]
	[Feature( INPUT ), Order( SPRINTING_ORDER )]
	[Range( 0f, 3f, clamped: false ), Step( 0.01f )]
	public virtual float SprintMultiplier { get; set; } = 1.5f;

	/// <summary>
	/// Should the owner be able to crouch?
	/// </summary>
	[Property]
	[Feature( INPUT ), Order( DUCKING_ORDER )]
	[ToggleGroup( nameof( AllowDucking ), Label = DUCKING )]
	public virtual bool AllowDucking { get; set; } = true;

	/// <summary>
	/// The button to crouch. <br />
	/// Set this to blank/null to disable it.
	/// </summary>
	[Property]
	[InputAction]
	[ToggleGroup( nameof( AllowDucking ) )]
	[Feature( INPUT ), Order( DUCKING_ORDER )]
	public virtual string DuckButton { get; set; } = "duck";
	public virtual bool HasDuckButton => !string.IsNullOrWhiteSpace( DuckButton );

	public virtual bool ShouldDuck => AllowDucking && HasDuckButton
		&& Input.Down( DuckButton );

	[Property]
	[Title( "Move Speed (Ducked)" )]
	[Range( 0f, 1000f, clamped: false )]
	[ToggleGroup( nameof( AllowDucking ) )]
	[Feature( INPUT ), Order( DUCKING_ORDER )]
	public virtual float MoveSpeedDucked { get; set; } = 120f;

	[Sync]
	public bool IsDucking { get; set; }

	/// <summary>
	/// Should the owner be able to jump?
	/// </summary>
	[Property]
	[Feature( INPUT ), Order( JUMPING_ORDER )]
	[ToggleGroup( nameof( AllowJumping ), Label = JUMPING )]
	public virtual bool AllowJumping { get; set; } = true;

	/// <summary>
	/// The button to let you jump. <br />
	/// Set this to blank/null to disable it.
	/// </summary>
	[Property]
	[InputAction]
	[ToggleGroup( nameof( AllowJumping ) )]
	[Feature( INPUT ), Order( JUMPING_ORDER )]
	public virtual string JumpButton { get; set; } = "jump";
	public virtual bool HasJumpButton => !string.IsNullOrWhiteSpace( JumpButton );

	public virtual bool ShouldJump => AllowJumping && HasJumpButton
		&& Input.Down( JumpButton );

	[Property]
	[ToggleGroup( nameof( AllowJumping ) )]
	[ShowIf( nameof( HasJumpButton ), true )]
	[Feature( INPUT ), Order( JUMPING_ORDER )]
	public virtual float JumpSpeed { get; set; } = 400f;

	public virtual Vector3 GetJumpVelocity()
		=> WorldRotation.Up * JumpSpeed;

	/// <summary>
	/// How quickly to transition towards the target position.
	/// </summary>
	[Property]
	[Title( "Speed" )]
	[Range( 0.1f, 5f, clamped: false ), Step( 0.01f )]
	[Feature( VIEW ), Group( EYEPOS ), Order( EYEPOS_ORDER )]
	public virtual float EyeMoveSpeed { get; set; } = 1f;

	/// <summary>
	/// Transition speed resistance.
	/// Helps smooth things out(as the name would imply).
	/// </summary>
	[Property]
	[Title( "Smoothing" )]
	[Range( 0f, 1f, clamped: false ), Step( 0.01f )]
	[Feature( VIEW ), Group( EYEPOS ), Order( EYEPOS_ORDER )]
	public virtual float EyeMoveSmoothing { get; set; } = 0.15f;

	[Property]
	[Title( "Standing Height" )]
	[Feature( VIEW ), Group( EYEPOS ), Order( EYEPOS_ORDER )]
	public virtual float EyeHeightStand { get; set; } = 64f;

	[Property]
	[Title( "Ducked Height" )]
	[Feature( VIEW ), Group( EYEPOS ), Order( EYEPOS_ORDER )]
	public virtual float EyeHeightDuck { get; set; } = 32f;

	protected Vector3 _eyeVel = Vector3.Zero;

	/// <summary>
	/// Should the owner's look input rotate their eye angles?
	/// </summary>
	[Property]
	[Feature( VIEW ), Order( AIMING_ORDER )]
	[ToggleGroup( value: nameof( AllowAiming ), Label = "Aiming" )]
	public virtual bool AllowAiming { get; set; } = true;

	[Property]
	[ToggleGroup( nameof( AllowAiming ) )]
	[Feature( VIEW ), Order( AIMING_ORDER )]
	public virtual bool PitchClamping { get; set; } = true;

	[Property]
	[Range( 0, 180 )]
	[ToggleGroup( nameof( AllowAiming ) )]
	[Feature( VIEW ), Order( AIMING_ORDER )]
	[ShowIf( nameof( PitchClamping ), true )]
	public virtual FloatRange PitchRange { get; set; } = new( -89.9f, 89.9f );

	/// <summary>
	/// An Euler angle representing where this view is looking.
	/// </summary>
	[Property]
	[ToggleGroup( nameof( AllowAiming ) )]
	[Feature( VIEW ), Order( AIMING_ORDER )]
	public virtual Angles LocalEyeAngles
	{
		get => LocalEyeRotation;
		set => LocalEyeRotation = PitchClamping
			? value.WithPitch( value.pitch.Clamp( PitchRange ) )
			: value;
	}

	protected Rotation _viewRotation = Rotation.Identity;

	[Sync( SyncFlags.Interpolate )]
	public Vector3 LocalEyePosition
	{
		get => GetLocalEyePosition();
		set => SetLocalEyePosition( value );
	}

	protected Vector3 _localEyePos;

	[Sync( SyncFlags.Interpolate )]
	public Rotation LocalEyeRotation
	{
		get => GetLocalEyeRotation();
		set => SetLocalEyeRotation( value );
	}

	protected Rotation _localEyeRotation = Rotation.Identity;

	public Vector3 EyeForward => WorldTransform.RotationToWorld( LocalEyeRotation ).Forward.Normal;

	public virtual Vector3 GetLocalEyePosition()
		=> _localEyePos;

	public virtual void SetLocalEyePosition( Vector3 value )
	{
		if ( ITransform.IsValid( in value ) )
			_localEyePos = value;
	}

	public virtual Rotation GetLocalEyeRotation()
		=> _localEyeRotation;

	public virtual void SetLocalEyeRotation( Rotation value )
	{
		if ( ITransform.IsValid( in value ) )
			_localEyeRotation = value;
	}

	public float EyeHeight
	{
		get => LocalEyePosition.z;
		set => LocalEyePosition = LocalEyePosition.WithZ( value );
	}

	public virtual Vector3 GetEyeTargetPosition()
		=> Vector3.Up * (IsDucking ? EyeHeightDuck : EyeHeightStand);

	protected override void OnStart()
	{
		base.OnStart();

		LocalEyePosition = GetEyeTargetPosition();
	}

	public void FrameSimulate( in float deltaTime )
	{
		DoAiming( in deltaTime );
		Move( in deltaTime );
	}

	public virtual float GetWishSpeed()
	{
		if ( !AllowMovement )
			return 0f;

		if ( !HasDuckButton )
			return MoveSpeed;

		return LocalEyePosition.z.Remap( EyeHeightDuck, EyeHeightStand, MoveSpeedDucked, MoveSpeed );
	}

	public virtual Vector3 GetWishDirection()
	{
		var up = WorldRotation.Up;

		var flatAim = Vector3.VectorPlaneProject( EyeForward, up );
		var rMove = Rotation.LookAt( flatAim, up );
		var moveInput = Input.AnalogMove.ClampLength( 1f );

		return rMove * moveInput;
	}

	public virtual Vector3 GetWishVelocity()
	{
		var wishSpeed = GetWishSpeed();

		if ( wishSpeed == 0f )
			return Vector3.Zero;

		return GetWishDirection() * wishSpeed;
	}

	public virtual void UpdateView( in float deltaTime )
	{
		if ( IsProxy )
			return;

		UpdateEyePosition( deltaTime );
	}

	/// <summary>
	/// Performs eye position target transitioning.
	/// </summary>
	public virtual void UpdateEyePosition( in float deltaTime )
	{
		LocalEyePosition = Vector3.SmoothDamp( LocalEyePosition, GetEyeTargetPosition(),
			ref _eyeVel, EyeMoveSmoothing, EyeMoveSpeed * deltaTime );
	}

	protected virtual void DoAiming( in float deltaTime )
	{
		var angLook = Input.AnalogLook;

		if ( PitchClamping )
		{
			Angles angAim = LocalEyeAngles;

			angAim.pitch = (angAim.pitch + angLook.pitch).Clamp( PitchRange );
			angAim.yaw += angLook.yaw;

			angAim.roll = angAim.roll.LerpDegreesTo( 0f, Time.Delta * 10f );

			LocalEyeAngles = angAim;
		}
		else
		{
			var rAim = LocalEyeRotation;
			var rInverse = rAim.Inverse;

			rAim *= Rotation.FromAxis( rInverse.Up, angLook.yaw );
			rAim *= Rotation.FromPitch( angLook.pitch );

			rAim *= Rotation.FromRoll( -rAim.Roll() * deltaTime * 10f );

			LocalEyeRotation = rAim;
		}
	}
}
