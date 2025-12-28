namespace Playground;

public partial class EditorObjectGroup : EditorObject
{
	/// <summary>
	/// Finds all child objects that we should be concerned with.
	/// </summary>
	public IEnumerable<EditorObject> FindObjects()
		=> Components?.GetAll<EditorObject>() ?? [];

	public virtual bool TryAddObject( EditorObject ent, Offset offset )
	{
		if ( !ent.IsValid() || !ent.GameObject.IsValid() )
			return false;

		// Can't add groups to groups(for now?).
		if ( ent is EditorObjectGroup )
			return false;

		ent.GameObject.SetParent( GameObject );

		if ( ent.GameObject.Parent != GameObject )
			return false;

		ent.SetOffset( offset );
		ent.Transform.ClearInterpolation();

		return true;
	}

	/// <summary>
	/// Something was just added to this.
	/// </summary>
	public virtual void OnObjectAdded( EditorObject ent )
	{
	}
}
