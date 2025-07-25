namespace GameFish;

partial class PawnView
{
	public bool HasThirdPersonMode => IsModeEnabled( Perspective.ThirdPerson );

	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public FloatRange CameraDistance { get; set; } = new( 50f, 500f );

	/// <summary>
	/// Sensitivity of the mouse wheel when used to change the distance. <br />
	/// Can be negative to invert the mouse wheel direction, or zero
	/// to disable the mouse wheel altogether.
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public float ScrollSensitivity { get; set; } = 5f;

	/// <summary>
	/// A value that is passed to <see cref="MathX.Lerp(float,float,float,bool)"/>. <br />
	/// If <c>null</c>: disable smoothing altogether 
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public float? ScrollSmoothing { get; set; } = 5f;

	/// <summary>
	/// Radius of the sphere collider that will prevent the camera from phasing through walls. <br />
	/// If <c>null</c>: disable the camera collisions.  
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public float CollisionRadius { get; set; } = 5f;

	/// <summary>
	/// Tags of objects that will obstruct the camera view.
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public TagSet CollisionTags { get; set; } = ["solid"];

	public float CurrentDistance { get; set; }
	public float DesiredDistance { get; set; }

	protected virtual void OnThirdPersonModeSet()
	{
	}

	protected virtual void SetThirdPersonModeTransform()
	{
		SetRelativeTransform();
	}

	protected virtual void OnThirdPersonModeUpdate( in float deltaTime )
	{
		var pawn = Pawn;

		if ( !pawn.IsValid() )
			return;

		DesiredDistance -= Input.MouseWheel.y * ScrollSensitivity * deltaTime;

		if ( ScrollSmoothing is { } mws )
			CurrentDistance = CurrentDistance.LerpTo( DesiredDistance, mws * deltaTime );

		if ( CollisionRadius is { } radius )
		{
			var startPos = pawn.EyePosition;
			var endPos = startPos - (pawn.EyeForward * DesiredDistance);

			var trace = Scene.Trace.Sphere( radius, startPos, endPos )
				.IgnoreGameObject( pawn.GameObject )
				.WithAnyTags( CollisionTags )
				.Run();

			if ( trace.Hit )
				CurrentDistance = CurrentDistance.Min( trace.Distance );

			// DebugOverlay.ScreenText( Vector2.One * 20,
			// 	$"{DesiredDistance} {trace.Distance} {trace.Hit} {trace.GameObject?.Name}",
			// 	flags: TextFlag.LeftTop );
			// DebugOverlay.Line( trace.StartPosition, trace.EndPosition, Color.Red, overlay: true );
		}

		// Also clamp the current distance, because it starts with 0
		CurrentDistance = CurrentDistance.Clamp( CameraDistance );

		var tPos = Vector3.Backward * 150f;

		Relative = new( tPos, Rotation.Identity );
	}
}
