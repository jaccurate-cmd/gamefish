namespace Playground;

partial class EditorObject
{
	public EditorObjectGroup ObjectGroup
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

	protected EditorObjectGroup _objGroup = null;

	protected virtual void OnSetObjectGroup( EditorObjectGroup newGroup, EditorObjectGroup oldGroup )
	{
		this.Log( $"group new:[{newGroup}] old:[{oldGroup}]" );
	}

	protected void RefreshGroup( GameObject parent = null )
	{
		parent ??= GameObject;

		if ( !parent.IsValid() )
		{
			ObjectGroup = null;
			return;
		}

		ObjectGroup = parent?.Components?.Get<EditorObjectGroup>( FindMode.Enabled | FindMode.InAncestors );

		if ( ObjectGroup.IsValid() )
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
