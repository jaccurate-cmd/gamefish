
namespace Playground;

public abstract partial class EditorToolFunction : PlaygroundModule
{
	protected const int EDITOR_ORDER = DEFAULT_ORDER - 1000;

	public override bool IsParent( ModuleEntity comp )
		=> comp is EditorTool;

	/// <summary>
	/// What button should be used? Is it on press/hold/release?
	/// </summary>
	[Title( "Input" )]
	[Feature( EDITOR ), Group( INPUT ), Order( EDITOR_ORDER )]
	[Property, InlineEditor( Label = false )]
	public virtual FunctionInput Input { get; set; } = new( "Attack1", InputMode.Pressed, 0.1f );

	public bool IsInputting => Input?.IsInputting() is true;

	public virtual void Simulate( in float deltaTime ) { }

	public virtual void Activate( in SceneTraceResult tr )
	{
	}
}
