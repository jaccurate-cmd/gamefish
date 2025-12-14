namespace Playground;

public class EditorTool : PlaygroundModule
{
	protected const int EDITOR_ORDER = DEFAULT_ORDER - 1000;

	public override bool IsParent( ModuleEntity comp )
		=> comp is Editor;

	public Editor Editor => Parent as Editor;

	[Property]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolName { get; set; } = "Tool";

	[Property, TextArea]
	[Feature( EDITOR ), Group( DISPLAY ), Order( EDITOR_ORDER )]
	public string ToolDescription { get; set; } = "Does stuff.";

	public virtual void OnEnter() { }
	public virtual void OnExit() { }

	public virtual void Simulate( in float deltaTime )
		=> DoFunctionActivation();

	protected void DoFunctionActivation()
	{
		foreach ( var func in GetModules<EditorToolFunction>() )
		{
			if ( !func.IsValid() || !func.Active )
				continue;

			if ( func.IsInputting )
				if ( TryTrace( out var tr ) )
					func.Activate( in tr );
		}
	}

	public virtual bool TryTrace( out SceneTraceResult tr )
		=> Editor.TryTrace( Scene, out tr );
}
