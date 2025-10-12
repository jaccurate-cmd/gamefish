using System;
using Sandbox.Network;

namespace GameFish;

/// <summary>
/// The networking manager. <br />
/// It spawns players and manages clients. <br />
/// <br />
/// <b> NOTE: </b> You are encouraged to inherit and override this component.
/// </summary>
[Icon( "dns" )]
public partial class Server : Singleton<Server>, Component.INetworkListener
{
	protected new const int DEBUG_ORDER = 9999;

	protected override bool? IsNetworkedOverride => true;

	[Property]
	[Feature( DEBUG ), Order( DEBUG_ORDER )]
	public bool IsServerActive => Networking.IsActive;

	[Property, Feature( Library.AGENT )]
	public PrefabFile PlayerClientPrefab { get; set; }

	[Property, Feature( Library.AGENT )]
	public PrefabFile PlayerPawnPrefab { get; set; }

	[Sync( SyncFlags.FromHost )]
	public NetList<Client> ClientList { get; set; }

	protected static IEnumerable<Client> AllClients => Instance?.ClientList ?? [];
	public static IEnumerable<Client> ValidClients => AllClients.Where( cl => cl.IsValid() );
	public static IEnumerable<Client> PlayerClients => ValidClients.Where( cl => cl.IsPlayer );
	public static IEnumerable<Client> ConnectedClients => PlayerClients.Where( cl => cl.Connected );

	[Sync( SyncFlags.FromHost )]
	public NetDictionary<Guid, Identity> IdentityHistory { get; set; }

	/// <summary>
	/// Time in seconds since the Unix epoch.
	/// </summary>
	public static long Time => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

	protected override void OnStart()
	{
		base.OnStart();

		Initialize();
	}

	/// <summary>
	/// Called whenever the scene starts(assuming it does with this component). <br />
	/// By default this starts a new lobby, but you can prevent this.
	/// </summary>
	public virtual void Initialize()
	{
		if ( Networking.IsHost && !Networking.IsActive )
			TryCreateLobby();
	}

	/// <summary>
	/// Allows you to configure the next lobby created. <br />
	/// This is a good place to apply user settings.
	/// </summary>
	/// <param name="context"> What kind of lobby? Could be an enum or a string. </param>
	/// <param name="privacy"> The privacy level. Typically defaults to public. </param>
	/// <returns> The new lobby's settings. </returns>
	public virtual LobbyConfig GetLobbyConfig( object context = null, LobbyPrivacy? privacy = null )
	{
		var cfg = new LobbyConfig();

		if ( privacy.HasValue )
			cfg.Privacy = privacy.Value;

		return cfg;
	}

	/// <summary>
	/// Attempts to start a new lobby of some kind.
	/// </summary>
	/// <remarks>
	/// Will use <see cref="GetLobbyConfig"/> if <paramref name="cfgOverride"/> is not specified.
	/// </remarks>
	/// <param name="context"> What kind of lobby? Could be an enum or a string. </param>
	/// <param name="cfgOverride"> The configuration to force. </param>
	/// <returns> If the new lobby could be created. </returns>
	public virtual bool TryCreateLobby( object context = null, LobbyConfig? cfgOverride = null )
	{
		if ( Networking.IsActive )
		{
			this.Warn( "Lobby creation failed: must close the active lobby first." );
			return false;
		}

		var cfg = cfgOverride ?? GetLobbyConfig();

		Networking.CreateLobby( cfg );

		return true;
	}

	public void OnActive( Connection cn )
	{
		if ( !Networking.IsHost )
		{
			this.Warn( $"{nameof( OnActive )} called on non-host!" );
			return;
		}

		if ( cn is null )
		{
			this.Warn( $"Null connection joined! [{cn}]" );
			return;
		}

		AssignClient( PlayerClientPrefab, cn );
	}

	public void OnDisconnected( Connection cn )
	{
		var cl = FindClient( cn );

		if ( cl.IsValid() )
			cl.DestroyGameObject();
	}

	/// <summary>
	/// Finds an existing(or creates and registers a new) <see cref="Client"/> object.
	/// </summary>
	protected virtual Client AssignClient( PrefabFile prefab, Connection cn = null )
		=> AssignClient<Client>( prefab, cn );

	/// <summary>
	/// Finds an existing(or creates and registers a new) <typeparamref name="TClient"/> object.
	/// </summary>
	protected virtual TClient AssignClient<TClient>( PrefabFile prefab, Connection cn = null ) where TClient : Client
	{
		if ( !Networking.IsHost )
		{
			this.Warn( $"tried to assign client prefab:[{prefab}] on non-host" );
			return null;
		}

		Client cl = FindClient( cn );

		// Create a new client if we couldn't find an existing one.
		if ( !cl.IsValid() )
		{
			if ( !prefab.TrySpawn( out var go ) )
			{
				this.Warn( $"Failed to spawn client prefab:[{prefab}]!" );
				return null;
			}

			cl = go.Components.Get<TClient>( FindMode.EverythingInSelf );

			if ( !cl.IsValid() )
			{
				this.Warn( $"Failed to find type:[{typeof( TClient )}] component on object:[{go}]!" );
				go.Destroy();

				return null;
			}

			cl.AssignConnection( cn, out _ );

			RegisterClient( cl );
		}

		// Spawn and assign their default pawn.
		if ( cl.IsValid() )
			cl.SetPawn( PlayerPawnPrefab );

		return cl as TClient;
	}

	protected virtual void RegisterClient( Client cl )
	{
		ClientList ??= [];

		if ( !cl.IsValid() || ClientList.Contains( cl ) )
			return;

		ClientList.Add( cl );

		ValidateClients();
	}

	protected virtual void ValidateClients()
	{
		if ( !Scene.IsValid() || ClientList is null )
			return;

		var toRemove = new List<Client>();

		foreach ( var cl in ClientList )
			if ( !cl.IsValid() || cl.Scene != Scene )
				toRemove.Add( cl );

		toRemove.ForEach( cl => { ClientList.Remove( cl ); cl.Destroy(); } );
	}

	/// <summary>
	/// Adds an <see cref="Identity"/> to the history by its connection Id.
	/// </summary>
	/// <param name="id"></param>
	public virtual void RegisterIdentity( ref Identity id )
	{
		if ( !id.IsValid() || id.Id == default )
			return;

		IdentityHistory ??= [];
		IdentityHistory[id.Id] = id;
	}
}
