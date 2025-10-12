namespace GameFish;

/// <summary>
/// A component you can access anywhere using its <see cref="Instance"/> property.
/// </summary>
/// <typeparam name="TComp"> The component you want to be a singleton. </typeparam>
public abstract class Singleton<TComp> : ModuleEntity where TComp : ModuleEntity
{
	protected override NetworkMode NetworkingModeDefault => NetworkMode.Object;
	protected override OwnerTransfer NetworkTransferModeDefault => OwnerTransfer.Fixed;
	protected override NetworkOrphaned NetworkOrphanedModeDefault => NetworkOrphaned.Host;

	public override Connection DefaultNetworkOwner => Connection.Host;

	[SkipHotload]
	private static TComp _instance;

	/// <summary>
	/// The first active non-editor instance of <typeparamref name="TComp"/>.
	/// </summary>
	public static TComp Instance
	{
		get => _instance.GetSingleton();
		protected set => _instance = value;
	}

	/// <summary>
	/// The first active instance of <typeparamref name="TComp"/>. <br />
	/// This works even in in the editor.
	/// </summary>
	public static TComp EditorInstance
	{
		get => _instance.GetSingleton( allowEditor: true );
		protected set => _instance = value;
	}
}
