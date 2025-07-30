using System.Threading.Tasks;

namespace GameFish;

partial class PawnView
{
	/// <summary>
	/// Updates and then returns the world transform of this view.
	/// </summary>
	/// <returns> Where the camera should be positioned. </returns>
	public virtual Transform GetViewTransform()
	{
		UpdateTransform();
		return WorldTransform;
	}

	/// <summary>
	/// Applies current offset/transition effects to the view's transform.
	/// </summary>
	public virtual void UpdateTransform()
	{
		UpdateModeTransform();
	}

	/// <summary>
	/// Allows this view to specify the origin from which it may offset from. <br />
	/// By default this is <see cref="TargetPawn"/>'s eye transform.
	/// </summary>
	/// <returns> The base transform to offset from. </returns>
	public virtual Transform GetOrigin()
	{
		var targetPawn = TargetPawn;

		var pawn = targetPawn.IsValid()
			? targetPawn
			: ParentPawn;

		if ( !pawn.IsValid() )
			return global::Transform.Zero;

		return pawn.EyeTransform;
	}

	/// <summary>
	/// Sets this view's transform according to the curent mode.
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
