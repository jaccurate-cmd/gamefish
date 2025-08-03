namespace GameFish;

partial class PawnView
{
	public virtual Vector3 ViewPosition
	{
		get => _viewPos;
		set { _viewPos = value; }
	}

	protected Vector3 _viewPos;

	public virtual Rotation ViewRotation
	{
		get => _viewRotation;
		set { _viewRotation = value; }
	}

	protected Rotation _viewRotation;

	public Vector3 ViewForward => ViewRotation.Forward;
	public Transform ViewTransform => new( ViewPosition, ViewRotation, WorldScale );

	public virtual Vector3 PawnEyePosition => TargetPawn?.EyePosition ?? ViewPosition;
	public virtual Rotation PawnEyeRotation => TargetPawn?.EyeRotation ?? ViewRotation;
	public virtual Vector3 PawnScale => TargetPawn?.WorldScale ?? Vector3.One;

	public virtual Transform PawnEyeTransform => new( PawnEyePosition, PawnEyeRotation, PawnScale );

	/// <summary>
	/// Distance from this view to the pawn's first-person origin.
	/// </summary>
	public float DistanceFromEye => ViewPosition.Distance( PawnEyePosition );

	/// <summary>
	/// Allows this view to specify the origin from which it may offset from. <br />
	/// By default this is the <see cref="TargetPawn"/>'s eye transform.
	/// </summary>
	/// <returns> The base transform to offset from. </returns>
	public virtual Transform GetViewOrigin()
		=> PawnEyeTransform;

	/// <summary>
	/// Updates and then returns the view transform.
	/// </summary>
	/// <param name="updateView"> If true: update where the camera should render from. </param>
	/// <param name="updateObject"> If true: update the view's GameObject world transform. </param>
	/// <returns> Where the camera should be positioned. </returns>
	public virtual Transform GetViewTransform( bool updateView = true, bool updateObject = true )
	{
		UpdateViewTransform( updateView, updateObject );

		return ViewTransform;
	}

	/// <summary>
	/// Applies current offset/transition effects to the view's world position/rotation. <br />
	/// Also allows (not) setting the world transform of the viewing object itself.
	/// </summary>
	/// <param name="updateView"> If true: update where the camera should render from. </param>
	/// <param name="updateObject"> If true: update the view's GameObject world transform. </param>
	public virtual void UpdateViewTransform( bool updateView = true, bool updateObject = true )
	{
		if ( updateView )
			UpdateModeTransform();

		if ( updateObject )
			TrySetTransform( ViewTransform );
	}

	/// <summary>
	/// Sets this view's transform according to the current mode.
	/// </summary>
	protected virtual void UpdateModeTransform()
	{
		switch ( Mode )
		{
			case Perspective.FirstPerson:
				SetFirstPersonModeTransform();
				break;

			case Perspective.ThirdPerson:
				SetThirdPersonModeTransform();
				break;

			case Perspective.FreeCam:
				SetFreeCamModeTransform();
				break;

			case Perspective.Fixed:
				SetFixedModeTransform();
				break;

			case Perspective.Manual:
				SetManualModeTransform();
				break;

			case Perspective.Custom:
				SetCustomModeTransform();
				break;
		}
	}

	protected virtual void SetFixedModeTransform()
	{
	}

	protected virtual void SetManualModeTransform()
	{
	}

	protected virtual void SetCustomModeTransform()
	{
	}
}
