namespace GameFish;

partial class PawnView
{
	/// <summary>
	/// How quickly to transition between modes.
	/// </summary>
	[Property]
	[Title( "Speed" )]
	[Feature( MODES ), Group( TRANSITIONING )]
	public float TransitionSpeed { get; set; } = 3f;

	/// <summary>
	/// The smoothness of mode transition speed. Slows it down effectively.
	/// </summary>
	[Property]
	[Title( "Smoothing" )]
	[Feature( MODES ), Group( TRANSITIONING )]
	public float TransitionSmoothing { get; set; } = 0.35f;

	/// <summary>
	/// The local position and rotation relative to the pawn's origin.
	/// </summary>
	[Title( "Relative Offset" )]
	[Property, ReadOnly, InlineEditor]
	[Feature( MODES ), Group( TRANSITIONING )]
	public Offset Relative
	{
		get => _relative;
		set
		{
			_relative = value;
			// UpdateTransform();
		}
	}

	protected Offset _relative;

	/// <summary>
	/// The previous relative position and rotation to transition from.
	/// </summary>
	public Offset? PreviousOffset { get; set; }

	/// <summary>
	/// The previous world position to transition from(optional).
	/// </summary>
	public Vector3? PreviousPosition { get; set; }

	public virtual float TransitionFraction
	{
		get => _transFrac;
		set => _transFrac = value.Clamp( 0f, 1f );
	}

	protected float _transFrac;

	public virtual float TransitionVelocity
	{
		get => _transVel;
		set => _transVel = value;
	}

	protected float _transVel;

	/// <summary>
	/// Begins a transition given the current position of this view.
	/// </summary>
	public virtual void StartTransition( in bool useWorldPosition = false )
	{
		var pawn = TargetPawn;

		if ( !pawn.IsValid() )
			return;

		var tWorld = WorldTransform;

		PreviousPosition = useWorldPosition ? WorldPosition : null;
		PreviousOffset = new( GetOrigin().ToLocal( tWorld ) );

		TransitionFraction = 0f;
		_transVel = 0f;
	}

	/// <summary>
	/// Instantly ends the transition. Might cause visible snapping if done midway!
	/// </summary>
	public virtual void StopTransition()
	{
		PreviousPosition = null;
		PreviousOffset = null;

		TransitionFraction = 1f;
	}

	protected virtual void UpdateTransition( in float deltaTime )
	{
		if ( !PreviousOffset.HasValue )
			return;

		TransitionFraction = MathX.SmoothDamp( TransitionFraction, 1f, ref _transVel, TransitionSmoothing, Time.Delta )
			.Clamp( 0f, 1f );

		if ( TransitionFraction.AlmostEqual( 1f ) )
		{
			TransitionFraction = 1f;

			PreviousPosition = null;
			PreviousOffset = null;
		}
	}

	/// <summary>
	/// Sets the transform safely using <see cref="Relative"/> with transition support.
	/// </summary>
	protected virtual void SetTransformFromRelative()
	{
		var tOrigin = GetOrigin();

		if ( PreviousOffset is Offset prevOffset )
		{
			// Smoothed transitioning.
			var offsLerped = prevOffset.LerpTo( Relative, TransitionFraction );

			var tLerped = offsLerped.ToWorld( tOrigin );

			if ( PreviousPosition is Vector3 prevPos )
			{
				var tRelative = Relative.ToWorld( tOrigin );
				tLerped.Position = prevPos.LerpTo( tRelative.Position, TransitionFraction );
			}

			TrySetPosition( tLerped.Position );
			TrySetRotation( tLerped.Rotation );
		}
		else
		{
			// No transitioning.
			var tRelative = Relative.ToWorld( tOrigin );

			TrySetPosition( tRelative.Position );
			TrySetRotation( tRelative.Rotation );
		}
	}
}
