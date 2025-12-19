namespace Playground;

partial class Editor : IFocus
{
	// Allow tool to decide if aim/movement is blocked.
	public virtual bool HasAimingFocus => Tool?.PreventAiming is true;
	public virtual bool HasMovingFocus => Tool?.PreventMoving is true;

	// Always prevent other actions(like attacking/ducking) while the menu is open.
	public virtual bool HasActionFocus => IsOpen;
}
