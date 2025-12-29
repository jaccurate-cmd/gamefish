using System;
using System.Runtime.CompilerServices;

namespace Playground;

/// <summary>
/// Data for trying to spawn something with a tool.
/// </summary>
public struct ObjectSpawnConfig : IValid
{
	public readonly bool IsValid => Prefab.IsValid()
		&& (!InParent || Island.IsValid())
		&& ITransform.IsValid( Transform );

	// TODO: Use IDs or resources for security?
	public PrefabFile Prefab { get; set; }

	/// <summary>
	/// Should we parent this to a group?
	/// This makes <see cref="Transform"/> relative.
	/// <br /> <br />
	/// <b> NOTE: </b> Enabling this means spawning fails if it can't enter the group.
	/// <br />
	/// This is ideal so that you don't spawn using a local transform in world-space.
	/// </summary>
	public bool InParent { get; set; }

	/// <summary>
	/// The group to put this object in.
	/// </summary>
	public EditorObjectGroup Island { get; set; }

	/// <summary>
	/// Local-space if <see cref="InParent"/>, otherwise in world.
	/// </summary>
	public Transform Transform { get; set; }

	public readonly bool IsTransformLocal => InParent;

	/// <summary>
	/// Should this start with its motion enabled?
	/// <br /> <br />
	/// <b> NOTE: </b> Not yet implemented.
	/// </summary>
	public bool Motion { get; set; } = true;

	public ObjectSpawnConfig() { }

	/// <summary>
	/// For spawning an entity on its own in world space.
	/// </summary>
	public ObjectSpawnConfig( PrefabFile prefab, in Transform tWorld )
	{
		Prefab = prefab;

		Transform = tWorld;

		InParent = false;
		Island = null;
	}

	/// <summary>
	/// For spawning an entity attached to other stuff.
	/// </summary>
	public ObjectSpawnConfig( PrefabFile prefab, EditorObjectGroup group, in Transform tLocal )
	{
		Prefab = prefab;

		Transform = tLocal;

		InParent = true;
		Island = group;
	}

	public static ObjectSpawnConfig FromWorld( PrefabFile prefab, EditorObjectGroup group, in Transform tWorld )
	{
		var tLocal = group.IsValid()
			? group.WorldTransform.ToLocal( tWorld )
			: Transform.Zero;

		var cfg = new ObjectSpawnConfig( prefab, group, tLocal );

		return cfg;
	}

	public ObjectSpawnConfig WithMotion( in bool isEnabled )
		=> this with { Motion = isEnabled };

	public readonly bool TryGetWorldTransform( out Transform tWorld )
	{
		if ( !ITransform.IsValid( Transform ) )
			goto Bad;

		if ( !InParent )
		{
			tWorld = Transform;
			return true;
		}

		if ( Island.IsValid() )
		{
			tWorld = Island.WorldTransform.ToLocal( Transform );
			return true;
		}

		Bad:

		tWorld = Transform.Zero;
		return false;
	}
}
