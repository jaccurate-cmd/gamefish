namespace GameFish;

partial class PawnView
{
	public virtual bool HasThirdPersonMode => IsModeAllowed( Perspective.ThirdPerson );

	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public virtual float? InitialDistance { get; set; } = 100f;

	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public virtual FloatRange DistanceRange { get; set; } = new( 50f, 300f );

	/// <summary>
	/// Sensitivity of the mouse wheel when used to change the distance. <br />
	/// Can be negative to invert the mouse wheel direction, or zero
	/// to disable the mouse wheel altogether.
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public virtual float ScrollSensitivity { get; set; } = 15f;

	/// <summary>
	/// How quickly scrolling is smoothed towards its intended distance. <br />
	/// If zero or less: disable smoothing altogether.
	/// </summary>
	[Property]
	[Feature( MODES )]
	[Range( 0f, 20f, clamped: false )]
	[Group( THIRD_PERSON ), Order( THIRD_PERSON_ORDER )]
	public virtual float ScrollSpeed { get; set; } = 5f;

	public virtual float CurrentDistance { get; set; }
	public virtual float DesiredDistance { get; set; }

	protected virtual void OnThirdPersonModeSet()
	{
		if ( InitialDistance.HasValue )
		{
			DesiredDistance = InitialDistance.Value;
			CurrentDistance = DesiredDistance;
		}
	}

	protected virtual void SetThirdPersonModeTransform()
	{
		SetTransformFromRelative();
	}

	protected virtual void OnThirdPersonModeUpdate( in float deltaTime )
	{
		var pawn = TargetPawn;

		if ( !pawn.IsValid() )
			return;

		var tOrigin = GetOrigin();
		var aimDir = tOrigin.Forward;

		DesiredDistance -= Input.MouseWheel.y * ScrollSensitivity;
		DesiredDistance = DesiredDistance.Clamp( DistanceRange );

		CurrentDistance = ScrollSpeed > 0f
			? CurrentDistance.LerpTo( DesiredDistance, ScrollSpeed * deltaTime )
			: DesiredDistance;

		if ( Collision )
		{
			var startPos = tOrigin.Position;
			var endPos = startPos - (aimDir * CurrentDistance);
			var radius = GetCollisionRadius();

			var trAll = Scene.Trace.Sphere( radius, startPos, endPos )
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
