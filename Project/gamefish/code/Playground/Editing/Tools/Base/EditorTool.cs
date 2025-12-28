using System.Runtime.InteropServices;

namespace Playground;

[Icon( "build" )]
public abstract partial class EditorTool : PlaygroundModule
{
	protected const int EDITOR_ORDER = DEFAULT_ORDER - 1000;

	protected const int INPUT_ORDER = EDITOR_ORDER + 25;
	protected const int SOUNDS_ORDER = EDITOR_ORDER + 50;
	protected const int PREFABS_ORDER = EDITOR_ORDER + 100;
	protected const int SETTINGS_ORDER = EDITOR_ORDER + 150;

	public const string DEFAULT_EMOJI = "⚪";

	public override bool IsParent( ModuleEntity comp )
		=> comp is Editor;

	public Editor Editor => Parent as Editor;

	public static Client Client => Client.Local;

	public bool IsMenuOpen => Editor.IsValid() && Editor.IsOpen;
	public bool ShowCursor => Editor.IsValid() && Editor.ShowCursor;

	public bool IsSelected => Editor.IsValid() && Editor.Tool == this;

	/// <summary>
	/// Restricts this tool the the host and/or authorized users only.
	/// </summary>
	[Property]
	[Feature( EDITOR ), Group( SECURITY ), Order( EDITOR_ORDER )]
	public bool IsAdminOnly { get; set; } = false;

	[Property]
	[Title( "Group" )]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public ToolType ToolType { get; set; } = ToolType.Default;

	[Property]
	[Title( "Emoji" )]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolEmoji { get; set; } = DEFAULT_EMOJI;

	[Property]
	[Title( "Name" )]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolName { get; set; } = "Tool";

	[Property, TextArea]
	[Title( "Description" )]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolDescription { get; set; } = "Does stuff.";

	/// <summary>
	/// The entity we're looking at.
	/// </summary>
	public Entity TargetEntity { get; protected set; }

	/// <summary>
	/// The relative position/rotation from the entity.
	/// </summary>
	public Offset? TargetEntityOffset { get; protected set; }

	/// <summary>
	/// The world-space location we're trying to put/do stuff.
	/// </summary>
	public Transform? TargetWorldTransform { get; set; }

	protected virtual Color ColorOutline => Color.Black.WithAlpha( 0.5f );
	protected virtual Color ColorFilled => Color.White.WithAlpha( 0.1f );
	protected virtual Color ColorArrow => Color.Black.WithAlpha( 0.8f );

	/// <summary>
	/// Quickly checks permision and gives you a client reference.
	/// </summary>
	/// <returns> If the connection(such as an RPC caller) is allowed to use this tool. </returns>
	protected bool TryUse( Connection cn, out Client cl )
		=> Server.TryFindClient( cn, out cl ) && IsClientAllowed( cl );

	public virtual bool IsClientAllowed( Client cl )
		=> !IsAdminOnly || cl?.Connection?.IsHost is true;

	public virtual void OnEnter() { }
	public virtual void OnExit() { }

	public virtual void FrameSimulate( in float deltaTime )
	{
	}

	public virtual void FixedSimulate( in float deltaTime )
	{
	}

	protected virtual void OnPrimary( in SceneTraceResult tr )
	{
	}

	protected virtual void OnSecondary( in SceneTraceResult tr )
	{
	}

	protected virtual void OnTertiary( in SceneTraceResult tr )
	{
	}

	protected virtual void OnReload( in SceneTraceResult tr )
	{
	}

	protected virtual void OnScroll( in Vector2 scroll )
	{
	}

	protected virtual void ClearTarget()
	{
		TargetEntity = null;
		TargetEntityOffset = null;
		TargetWorldTransform = null;
	}

	protected virtual void UpdateTarget( in float deltaTime )
	{
		ClearTarget();

		if ( !TryTrace( out var tr ) )
			return;

		if ( !IsClientAllowed( Client.Local ) )
			return;

		if ( !TryGetTarget( in tr, out var target ) )
			return;

		TrySetTarget( target, in tr );
	}

	protected virtual bool TryGetTarget( in SceneTraceResult tr, out Entity target )
	{
		target = null;

		if ( !tr.Hit || !tr.GameObject.IsValid() )
			return false;

		const FindMode findMode = FindMode.EnabledInSelf
			| FindMode.InAncestors;

		target = tr.GameObject.Components.GetAll<Entity>( findMode )
			.Where( IsValidTarget )
			.FirstOrDefault();

		return target.IsValid();
	}

	public virtual bool TrySetTarget( Entity target, in SceneTraceResult tr )
	{
		if ( !IsValidTarget( target ) )
			return false;

		TargetEntity = target;

		return true;
	}

	public virtual bool IsValidTarget( Entity ent )
		=> ent.IsValid();
}
