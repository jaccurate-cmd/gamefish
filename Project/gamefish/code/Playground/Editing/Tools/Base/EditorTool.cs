namespace Playground;

[Icon( "build" )]
public abstract class EditorTool : PlaygroundModule
{
	protected const int EDITOR_ORDER = DEFAULT_ORDER - 1000;

	protected const int INPUT_ORDER = EDITOR_ORDER + 50;
	protected const int SETTINGS_ORDER = EDITOR_ORDER + 100;

	public override bool IsParent( ModuleEntity comp )
		=> comp is Editor;

	public Editor Editor => Parent as Editor;

	/// <summary>
	/// Restricts this tool the the host and/or authorized users only.
	/// </summary>
	[Property]
	[Feature( EDITOR ), Group( SECURITY ), Order( EDITOR_ORDER )]
	public bool IsAdminOnly { get; set; } = false;

	[Property]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolName { get; set; } = "Tool";

	[Property, TextArea]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolDescription { get; set; } = "Does stuff.";

	[Property, WideMode]
	[Feature( EDITOR ), Group( INPUT ), Order( INPUT_ORDER )]
	public List<ToolFunction> FunctionHints { get; set; }

	[Property]
	[Feature( EDITOR ), Group( SETTINGS ), Order( SETTINGS_ORDER )]
	public TraceFilter Filter { get; set; }

	public virtual bool IsAllowed( Connection cn )
		=> !IsAdminOnly || cn?.IsHost is true;

	public virtual void OnEnter() { }
	public virtual void OnExit() { }

	public virtual void FrameSimulate( in float deltaTime )
	{
	}

	public virtual void FixedSimulate( in float deltaTime )
	{
	}

	public virtual bool TryTrace( out SceneTraceResult tr )
		=> Editor.TryTrace( Scene, out tr );
}
