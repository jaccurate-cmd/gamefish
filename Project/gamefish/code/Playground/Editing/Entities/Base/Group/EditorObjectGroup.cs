namespace Playground;

public partial class EditorObjectGroup : EditorObject
{
	protected override void OnStart()
	{
		base.OnStart();

		CreatePhysics();
	}

	protected virtual void CreatePhysics()
	{
		if ( !this.InGame() )
			return;

		var rb = Components.Get<Rigidbody>( FindMode.EverythingInSelfAndAncestors );

		if ( rb.IsValid() )
			rb.Destroy();

		rb = Components.GetOrCreate<Rigidbody>();

		// 'cause uhhh fuck you, also bugs n shit
		// if ( rb.PhysicsBody.IsValid() )
			// rb.PhysicsBody.EnhancedCcd = true;

		// none of this shit please
		rb.EnableImpactDamage = false;
	}

	[Rpc.Broadcast]
	public void RpcBroadcastRefreshPhysics()
	{
		if ( !this.InGame() )
			return;

		if ( !Rigidbody.IsValid() )
		{
			CreatePhysics();
			return;
		}

		/*
		var colliders = Components.GetAll<Collider>( FindMode.EnabledInSelfAndDescendants )
			.Where( c => c.Enabled );

		if ( !colliders.Any() )
			return;

		foreach ( var c in colliders.ToArray() )
		{
			c.Enabled = false;
			c.Enabled = true;
		}

		if ( Rigidbody.IsValid() )
		{
			Rigidbody.Enabled = false;
			Rigidbody.Enabled = true;
		}
		*/
	}

	/// <summary>
	/// Finds all objects within that we should be concerned with.
	/// </summary>
	public IEnumerable<EditorObject> FindObjects()
		=> Components?.GetAll<EditorObject>( FindMode.Enabled | FindMode.InDescendants ) ?? [];

	/// <summary>
	/// Finds all objects within that we should be concerned with.
	/// </summary>
	public IEnumerable<Rigidbody> FindRigidbodies()
		=> Components?.GetAll<Rigidbody>( FindMode.InDescendants ) ?? [];

	public virtual bool TryAddObject( EditorObject ent, Offset offset )
	{
		if ( !ent.IsValid() || !ent.GameObject.IsValid() )
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
		RpcBroadcastRefreshPhysics();
	}
}
