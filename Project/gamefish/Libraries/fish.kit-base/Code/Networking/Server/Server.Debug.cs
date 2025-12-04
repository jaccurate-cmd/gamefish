using System.Text.Json.Serialization;

namespace GameFish;

partial class Server
{
	protected const int DEBUG_CLIENTS_ORDER = DEBUG_ORDER + 10;
	protected const int DEBUG_PAWNS_ORDER = DEBUG_ORDER + 20;

	/// <summary>
	/// Shows if <see cref="Networking.IsActive"/> is <c>true</c>.
	/// </summary>
	[Property]
	[Feature( DEBUG ), Group( NETWORKING ), Order( DEBUG_ORDER )]
	public bool IsNetworkingActive => Networking.IsActive;

	[Title( "Clients" )]
	[Property, ReadOnly, JsonIgnore]
	[ShowIf( nameof( InGame ), true )]
	[Feature( DEBUG ), Group( CLIENTS ), Order( DEBUG_CLIENTS_ORDER )]
	protected List<Client> InspectorAllClients => [.. AllClients];

	[Title( "Pawns" )]
	[Property, ReadOnly, JsonIgnore]
	[ShowIf( nameof( InGame ), true )]
	[Feature( DEBUG ), Group( PAWNS ), Order( DEBUG_PAWNS_ORDER )]
	protected List<Pawn> InspectorActivePawns => [.. Pawn.GetAllActive()];

	/// <summary>
	/// Looks for a <see cref="Server"/> singleton to see if cheating is globally enabled.
	/// If one wasn't found it will check <see cref="Application.CheatsEnabled"/> instead.
	/// </summary>
	public static bool CheatsEnabled => Instance is var sv && sv.IsValid()
		? sv.IsCheatingEnabled is true
		: Application.CheatsEnabled;

	/// <summary>
	/// Allows custom logic to enable cheats(such as a sandbox mode).
	/// Defaults to checking <see cref="Application.CheatsEnabled"/>.
	/// </summary>
	public virtual bool IsCheatingEnabled => Application.CheatsEnabled;

	/// <returns> If this particular connection is allowed to cheat. </returns>
	public static bool CanCheat( Connection cn )
		=> Instance?.AllowCheating( cn ) is true;

	/// <returns> If this particular connection is allowed to cheat. </returns>
	public virtual bool AllowCheating( Connection cn )
		=> IsCheatingEnabled;

	/// <summary>
	/// Cleans up all disconnected player clients.
	/// </summary>
	[Button]
	[Feature( DEBUG ), Group( CLIENTS ), Order( DEBUG_CLIENTS_ORDER - 1 )]
	public virtual void ValidateClients()
	{
		foreach ( var cl in PlayerClients.ToArray() )
			if ( !cl.Connected )
				cl.DestroyGameObject();
	}
}
