namespace GameFish;

partial class PawnView
{
	public bool HasThirdPersonMode => IsModeEnabled( Perspective.ThirdPerson );

	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public FloatRange DistanceRange { get; set; } = new( 50f, 500f );

	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public float? InitialDistance { get; set; } = 100f;

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
	/// If <c>0 or less</c>: disable smoothing altogether 
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Range( 0f, 20f, clamped: false )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public float ScrollSmoothing { get; set; } = 5f;

	public float CurrentDistance { get; set; }
	public float DesiredDistance { get; set; }

	protected virtual void OnThirdPersonModeSet()
	{
		if ( InitialDistance.HasValue )
			DesiredDistance = InitialDistance.Value;
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

		var aimDir = EyeForward;

		DesiredDistance -= Input.MouseWheel.y * ScrollSensitivity * deltaTime;
		DesiredDistance.Clamp( DistanceRange );

		if ( ScrollSmoothing > 0f )
			CurrentDistance = CurrentDistance.LerpTo( DesiredDistance, ScrollSmoothing * deltaTime );

		if ( Collision )
		{
			var startPos = EyePosition;
			var endPos = startPos - (aimDir * CurrentDistance);

			var trAll = Scene.Trace.Sphere( CollisionRadius, startPos, endPos )
				.IgnoreGameObject( pawn.GameObject )
				.WithAnyTags( CollisionHitTags )
				.WithoutTags( CollisionIgnoreTags )
				.RunAll();

			foreach ( var tr in trAll )
			{
				if ( !tr.Hit || (!CollideOwned && tr.GameObject.IsOwner()) )
					continue;

				CurrentDistance = CurrentDistance.Min( tr.Distance );
			}

			/*
			DebugOverlay.ScreenText( Vector2.One * 20,
				$"{DesiredDistance} {trace.Distance} {trace.Hit} {trace.GameObject?.Name}",
				flags: TextFlag.LeftTop );

			DebugOverlay.Line( trace.StartPosition, trace.EndPosition, Color.Red, overlay: true );
			*/
		}

		var tPos = Vector3.Backward * CurrentDistance;

		Relative = new( tPos, Rotation.Identity );
	}
}
