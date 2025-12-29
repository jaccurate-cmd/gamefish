namespace Playground;

partial class EditorObject
{
	public EditorIsland Island
	{
		get => _objGroup;
		protected set
		{
			if ( _objGroup == value )
				return;

			var old = _objGroup;
			_objGroup = value;

			OnSetObjectGroup( _objGroup, old );
		}
	}

	protected EditorIsland _objGroup = null;

	protected virtual void OnSetObjectGroup( EditorIsland newGroup, EditorIsland oldGroup )
	{
		this.Log( $"group new:[{newGroup}] old:[{oldGroup}]" );

		if ( newGroup.IsValid() )
			newGroup.OnObjectAdded( this );
	}

	protected void UpdateIsland( GameObject parent = null )
	{
		parent ??= GameObject;

		if ( !parent.IsValid() )
		{
			Island = null;
			return;
		}

		Island = parent?.Components?.Get<EditorIsland>( FindMode.Enabled | FindMode.InAncestors );

		if ( Island.IsValid() )
			DestroyPhysics();
	}

	protected void DestroyPhysics()
	{
		if ( !GameObject.IsValid() )
			return;

		var bodies = Components.GetAll<Rigidbody>( FindMode.EverythingInSelfAndDescendants );

		if ( !bodies.Any() )
			return;

		foreach ( var rb in bodies.ToArray() )
			rb.Destroy();
	}
}
