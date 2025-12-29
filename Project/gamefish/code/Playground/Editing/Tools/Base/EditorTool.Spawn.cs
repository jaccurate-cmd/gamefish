namespace Playground;

partial class EditorTool
{
	public static EditorObjectGroup FindIsland( GameObject obj )
	{
		if ( !obj.IsValid() )
			return null;

		return obj.Components?.Get<EditorObjectGroup>( FindMode.EnabledInSelf | FindMode.InAncestors );
	}

	public static bool TryGetObjectGroup( GameObject obj, out EditorObjectGroup group )
		=> (group = FindIsland( obj )).IsValid();

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

		return true;
	}

	public bool TrySpawnObject( PrefabFile prefab, in Transform tWorld, out EditorObject e )
		=> TrySpawnObject( new( prefab, tWorld ), out e );

	public bool TrySpawnObject( PrefabFile prefab, in Transform tWorld, GameObject objParent, out EditorObject e )
		=> TrySpawnObject( prefab, tWorld, FindIsland( objParent ), out e );

	public bool TrySpawnObject( PrefabFile prefab, in Transform tWorld, EditorObjectGroup parent, out EditorObject e )
	{
		var cfg = ObjectSpawnConfig.FromWorld( prefab, parent, tWorld );
		return TrySpawnObject( in cfg, out e );
	}

	public virtual bool TrySpawnObject( in ObjectSpawnConfig cfg, out EditorObject e )
	{
		e = null;

		// Permission check.
		if ( !IsClientAllowed( Client.Local ) )
			return false;

		if ( !cfg.IsValid() )
			return false;

		// Clone the prefab.
		if ( !cfg.TryGetWorldTransform( out var tWorld ) )
			return false;

		if ( !cfg.Prefab.TrySpawn( tWorld, out var eObj ) )
			return false;

		if ( !eObj.Components.TryGet( out e, FindMode.EnabledInSelf ) )
		{
			this.Warn( $"No {typeof( EditorObject )} on prefab:[{cfg.Prefab}]!" );

			eObj.DestroyImmediate();
			return false;
		}

		eObj.Transform.ClearInterpolation();
		e.SetupNetworking( force: true );

		// eObj.SetParent( parent.GameObject, keepWorldPosition: true );

		OnObjectSpawned( e, parent: null );

		// if ( parent.IsValid() )
			// parent.RpcBroadcastRefreshPhysics();

		return true;
	}

	protected virtual void OnObjectSpawned( EditorObject e, EditorObjectGroup parent )
	{
	}
}
