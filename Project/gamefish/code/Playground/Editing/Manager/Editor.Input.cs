namespace Playground;

partial class Editor : IFocus
{
	// Allow tool to decide if inputs are blocked.
	public virtual bool HasAimingFocus => Tool?.PreventAiming is true;
	public virtual bool HasMovingFocus => Tool?.PreventMoving is true;
	public virtual bool HasActionFocus => Tool?.PreventAction is true;
}
