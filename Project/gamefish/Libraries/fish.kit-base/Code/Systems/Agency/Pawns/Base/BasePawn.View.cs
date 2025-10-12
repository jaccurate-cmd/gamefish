namespace GameFish;

partial class BasePawn
{
	/// <summary>
	/// The central view manager for the pawn.
	/// </summary>
	[Property]
	[Feature( PAWN ), Group( VIEW )]
	public PawnView View
	{
		get => _view.IsValid() ? _view
			: _view = GetModule<PawnView>();

		set { _view = value; }
	}

	protected PawnView _view;

	[Property]
	[Feature( PAWN ), Group( VIEW )]
	public ViewRenderer ViewRenderer => View?.ViewRenderer;

	/// <summary> The base vision trace will ignore objects with these tags. </summary>
	[Property]
	[Feature( PAWN ), Group( VIEW )]
	public virtual TagSet EyeTraceIgnore { get; set; } = ["water"];

	/// <summary>
	/// The local eye position of the controller in world space.
	/// </summary>
	public virtual Vector3 EyePosition
	{
		get => WorldTransform.PointToWorld( Controller?.LocalEyePosition ?? LocalPosition );
		set
		{
			if ( Controller is var c && c.IsValid() )
				c.LocalEyePosition = WorldTransform.PointToLocal( value );
		}
	}

	/// <summary>
	/// The local eye rotation of the controller in world space.
	/// </summary>
	public virtual Rotation EyeRotation
	{
		get => WorldTransform.RotationToWorld( Controller?.LocalEyeRotation ?? LocalRotation );
		set
		{
			if ( Controller is var c && c.IsValid() )
				c.LocalEyeRotation = WorldTransform.RotationToLocal( value );
		}
	}

	public Transform EyeTransform => new( EyePosition, EyeRotation, WorldScale );
	public Vector3 EyeForward => EyeRotation.Forward;

	/// <summary>
	/// A position between the eye and feet.
	/// </summary>
	public override Vector3 Center => WorldPosition.LerpTo( EyePosition, 0.5f );

	/// <summary>
	/// Tells the view manager to process its transitions and offsets.
	/// </summary>
	public virtual void SimulateView( in float deltaTime )
	{
		if ( View is var view && view.IsValid() )
			view.FrameSimulate( deltaTime );
	}

	/// <summary>
	/// Lets this pawn affect/override a view transform(probably from the main camera).
	/// </summary>
	public virtual bool TryApplyView( CameraComponent cam, ref Transform tView )
	{
		// Allow view transform transitioning and effects.
		if ( View is PawnView view && view.IsValid() )
		{
			tView = view.GetViewTransform();
			return true;
		}

		// Default to the pawn's viewing origin.
		tView = EyeTransform;

		return true;
	}

	/// <summary>
	/// Called when the view targeting this pawn has finished updating. <br />
	/// It may be a view spectating this pawn, thus not the child view. <br />
	/// You should process clientside effects like model fading here.
	/// </summary>
	public virtual void OnViewUpdate( PawnView view )
	{
		if ( !view.IsValid() )
			return;

		if ( BodyComponent is PawnBody body && body.IsValid() )
			body.OnViewUpdate( view );
	}

	/// <returns> Prepares a default trace with vision filters. </returns>
	public virtual SceneTrace GetEyeTrace()
		=> Scene.Trace
			.IgnoreGameObjectHierarchy( GameObject )
			.WithoutTags( EyeTraceIgnore );

	/// <returns> Prepares a vision trace from the eye position to a point. </returns>
	public SceneTrace GetEyeTrace( Vector3 to )
		=> GetEyeTrace().FromTo( EyePosition, to );

	/// <returns> Prepares a vision trace from one point to another. </returns>
	public SceneTrace GetEyeTrace( Vector3 from, Vector3 to )
		=> GetEyeTrace().FromTo( from, to );

	/// <returns> Prepares a vision trace from the eye position. </returns>
	public SceneTrace GetEyeTrace( float distance, Vector3? dir = null )
	{
		var startPos = EyePosition;
		var endPos = startPos + ((dir ?? EyeForward) * distance);

		var tr = GetEyeTrace()
			.FromTo( startPos, endPos );

		return tr;
	}
}
