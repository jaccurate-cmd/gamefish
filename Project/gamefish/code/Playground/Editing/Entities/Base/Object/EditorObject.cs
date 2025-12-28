using System.Text.Json.Serialization;

namespace Playground;

/// <summary>
/// A physical object a player can create.
/// Tracks who and what it belongs to so that stuff can be managed.
/// </summary>
public partial class EditorObject : PhysicsObject
{
	protected const int EDITOR_ORDER = DEFAULT_ORDER - 1000;

	[Title( "Owner" )]
	[Property, JsonIgnore, ReadOnly]
	[ShowIf( nameof( InGame ), true )]
	[Feature( EDITOR ), Group( ID ), Order( EDITOR_ORDER - 100 )]
	protected SteamId? InspectorOwner
	{
		get => Owner;
		set => Owner = value;
	}

	/// <summary>
	/// The client this entity belongs to.
	/// Probably the one that spawn/requested it.
	/// </summary>
	[Sync( SyncFlags.FromHost )]
	public SteamId? Owner { get; protected set; }

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( !this.InGame() || !GameObject.IsValid() )
			return;

		// Auto-cleanup objects that don't have any other entities.
		const FindMode findMode = FindMode.EnabledInSelfAndDescendants;

		var comps = GameObject.Components.GetAll<ModuleEntity>( findMode )
			.Where( comp => comp.IsValid() );

		if ( !comps.Any() )
			GameObject.Destroy();
	}

	protected override void OnParentChanged( GameObject oldParent, GameObject newParent )
	{
		base.OnParentChanged( oldParent, newParent );

		RefreshGroup( newParent );
	}

	public virtual void RenderHelpers()
	{
	}
}
