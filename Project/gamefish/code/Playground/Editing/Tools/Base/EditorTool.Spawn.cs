namespace Playground;

partial class EditorTool
{
	public static EditorObjectGroup GetObjectGroup( GameObject obj )
	{
		if ( !obj.IsValid() )
			return null;

		return obj.Components?.Get<EditorObjectGroup>( FindMode.EnabledInSelf | FindMode.InAncestors );
	}

	public static bool TryGetObjectGroup( GameObject obj, out EditorObjectGroup group )
		=> (group = GetObjectGroup( obj )).IsValid();

	public virtual bool TryCreateObjectGroup( in Transform tWorld, out EditorObjectGroup group )
	{
		group = null;

		if ( !Scene.InGame() )
			return false;

		var objGroup = Scene.CreateObject( enabled: false );

		if ( !objGroup.IsValid() )
			return false;

		objGroup.Name = "Object Group";
		objGroup.WorldTransform = tWorld.WithScale( Vector3.One );

		group = objGroup.Components.Create<EditorObjectGroup>();

		if ( !group.IsValid() )
		{
			objGroup.DestroyImmediate();
			return false;
		}

		var rb = objGroup.Components.Create<Rigidbody>( startEnabled: true );

		if ( !rb.IsValid() )
		{
			objGroup.DestroyImmediate();
			return false;
		}

		return true;
	}

	/// <summary>
	/// Spawns a prefab safely and with auto-configuration.
	/// </summary>
	public bool TrySpawnObject( PrefabFile prefab, Transform tWorld, out GameObject obj )
		=> TrySpawnObject( prefab, parent: null, tWorld, out obj );

	/// <summary>
	/// Spawns a prefab safely and with auto-configuration.
	/// </summary>
	public bool TrySpawnObject( PrefabFile prefab, GameObject objParent, Transform tWorld, out GameObject obj )
		=> TrySpawnObject( prefab, GetObjectGroup( objParent ), tWorld, out obj );

	/// <summary>
	/// Spawns a prefab safely and with auto-configuration.
	/// </summary>
	public virtual bool TrySpawnObject( PrefabFile prefab, EditorObjectGroup parent, Transform tWorld, out GameObject eObj )
	{
		eObj = null;

		// Permission check.
		if ( !IsClientAllowed( Client.Local ) )
			return false;

		// Create a new object(entity) group if one wasn't specified.
		if ( !parent.IsValid() )
		{
			if ( !TryCreateObjectGroup( tWorld, out parent ) )
				return false;
		}

		// Create the prefab.
		if ( !prefab.TrySpawn( tWorld, out eObj ) )
		{
			parent.GameObject.DestroyImmediate();
			return false;
		}

		if ( !eObj.Components.TryGet<EditorObject>( out var e, FindMode.EnabledInSelf ) )
		{
			this.Warn( $"No {typeof( EditorObject )} on prefab:[{prefab}]!" );

			eObj.DestroyImmediate();
			parent.GameObject.DestroyImmediate();
			return false;
		}

		eObj.SetParent( parent.GameObject, keepWorldPosition: true );
		eObj.Transform.ClearInterpolation();

		e.SetupNetworking( force: true );
		parent.SetupNetworking( force: true );

		OnObjectSpawned( parent, e );

		return true;
	}

	protected virtual void OnObjectSpawned( EditorObjectGroup parent, EditorObject e )
	{
	}
}
