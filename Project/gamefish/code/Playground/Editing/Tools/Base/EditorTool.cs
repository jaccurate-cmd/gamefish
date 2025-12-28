using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace Playground;

[Icon( "build" )]
public abstract partial class EditorTool : PlaygroundModule
{
	protected const int EDITOR_ORDER = DEFAULT_ORDER - 1000;
	protected const int EDITOR_DEBUG_ORDER = EDITOR_ORDER - 100;

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
	public Offset? TargetOffset { get; protected set; }

	/// <summary>
	/// The world-space location we're trying to put/do stuff.
	/// </summary>
	public Transform? TargetWorldTransform { get; set; }

	/// <summary>
	/// The last targeting trace attempt.
	/// </summary>
	public SceneTraceResult? TargetTrace { get; set; }

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

	public virtual void OnEnter()
	{
		ClearTarget();
	}

	public virtual void OnExit() { }

	public virtual void FrameSimulate( in float deltaTime )
	{
		UpdateTarget( in deltaTime );
		UpdateActions( in deltaTime );

		RenderHelpers();
	}

	public virtual void FixedSimulate( in float deltaTime )
	{
	}

	/// <summary>
	/// Resets the tool's operating parameters.
	/// </summary>
	protected virtual void Clear()
	{
		ClearTarget();
	}

	/// <summary>
	/// Do stuff directly from action buttons being pressed.
	/// </summary>
	protected virtual void UpdateActions( in float deltaTime )
	{
		// Panel sends left/right click events.
		// TODO: Not this. Something less stupid.
		if ( !IsMenuOpen )
		{
			if ( PressedPrimary )
				if ( TryTrace( out var tr ) )
					OnPrimary( in tr );

			if ( PressedSecondary )
				if ( TryTrace( out var tr ) )
					OnSecondary( in tr );
		}

		if ( PressedReload )
			if ( TryTrace( out var tr ) )
				OnReload( in tr );

		if ( PressedUse )
			if ( TryTrace( out var tr ) )
				OnUse( in tr );
	}

	protected virtual void OnPrimary( in SceneTraceResult tr )
	{
	}

	protected virtual void OnSecondary( in SceneTraceResult tr )
	{
	}

	/// <summary>
	/// Like, middle mouse button or something? Probably.
	/// </summary>
	protected virtual void OnTertiary( in SceneTraceResult tr )
	{
	}

	protected virtual void OnUse( in SceneTraceResult tr )
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
		TargetOffset = null;
		TargetWorldTransform = null;

		TargetTrace = null;
	}

	/// <summary>
	/// Figure out what we're looking at right now.
	/// </summary>
	protected virtual void UpdateTarget( in float deltaTime, bool clearPrevious = true )
	{
		if ( clearPrevious )
			ClearTarget();

		if ( !IsClientAllowed( Client.Local ) )
			return;

		if ( !TryTrace( out var tr ) )
			return;

		TargetTrace = tr;

		if ( !TryGetTarget( in tr, out _ ) )
			return;
	}

	public virtual bool TrySetTarget( in SceneTraceResult tr, Component target )
	{
		if ( !target.IsValid() )
			return false;

		TargetTrace = tr;

		var tWorld = target.WorldTransform;
		TargetWorldTransform = tWorld;

		var vLocal = tWorld.PointToLocal( tr.EndPosition );
		var rLocal = tWorld.RotationToLocal( Rotation.LookAt( tr.Direction ) );
		TargetOffset = new( vLocal, rLocal );

		if ( target is Entity ent )
			TargetEntity = ent;

		return true;
	}

	public virtual bool TryGetTarget( in SceneTraceResult tr, out Component target )
	{
		if ( TryGetTargetEntity( in tr, out var ent ) )
		{
			target = ent;
			return true;
		}

		if ( tr.Collider.IsValid() && tr.Collider.Static is true )
		{
			target = tr.Collider;
			return true;
		}

		target = null;
		return false;
	}

	protected virtual bool TryGetTargetEntity( in SceneTraceResult tr, out Entity ent )
	{
		ent = null;

		if ( !tr.Hit || !tr.GameObject.IsValid() )
			return false;

		const FindMode findMode = FindMode.EnabledInSelf
			| FindMode.InAncestors;

		ent = tr.GameObject.Components.GetAll<Entity>( findMode )
			.Where( IsValidTarget )
			.FirstOrDefault();

		return ent.IsValid();
	}

	public virtual bool IsValidTarget( Entity ent )
		=> ent.IsValid();
}
