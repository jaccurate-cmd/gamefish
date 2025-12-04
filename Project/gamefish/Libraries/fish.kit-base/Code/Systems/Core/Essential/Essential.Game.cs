namespace GameFish;

partial class Essential
{
	protected const int GAME_ORDER = BOOT_ORDER + 100;
	protected const int GAME_DISPLAY_ORDER = GAME_ORDER - 1;
	protected const int GAME_PREFABS_ORDER = GAME_ORDER + 5;

	/// <summary>
	/// What's your game called?
	/// </summary>
	[Property]
	[Title( "Name" )]
	[Feature( GAME ), Group( DISPLAY ), Order( GAME_DISPLAY_ORDER )]
	protected virtual string GameName { get; set; }

	/// <summary>
	/// What version is your game at?
	/// </summary>
	[Property]
	[Title( "Version" )]
	[Feature( GAME ), Group( DISPLAY ), Order( GAME_DISPLAY_ORDER )]
	protected virtual string GameVersion { get; set; } = "0.1a";

	/// <summary>
	/// The prefab with a <see cref="GameManager"/> component on it.
	/// If defined here then it will be spawned at each
	/// scene start if one does not already exist.
	/// <br /> <br />
	/// <b> NOTE: </b> Has state modules for both main menu
	/// and ingame scenes to handle logic for each of them.
	/// </summary>
	[Property]
	[Title( "Game Manager" )]
	[Feature( GAME ), Group( PREFABS ), Order( GAME_PREFABS_ORDER )]
	public virtual PrefabFile GameManagerPrefab { get; set; }

	/// <summary>
	/// Spawns the <see cref="GameManager"/> prefab if an instance doesn't exist.
	/// </summary>
	[Button( "Ensure Manager" )]
	[ShowIf( nameof( InGame ), true )]
	[Feature( GAME ), Group( PREFABS ), Order( GAME_PREFABS_ORDER )]
	public virtual void EnsureGameManager()
	{
		if ( !this.InGame() || !Networking.IsHost )
			return;

		if ( GameManager.TryGetInstance( out _ ) )
			return;

		var prefab = GameManagerPrefab;

		// Scene settings might override the prefab.
		if ( SceneSettings.TryGetInstance( out var s ) )
			if ( s.GameManagerPrefabOverride.IsValid() )
				prefab = s.GameManagerPrefabOverride;

		if ( prefab.IsValid() )
			GameManager.TryCreate( prefab, out var _ );
	}
}
