namespace GameFish;

partial class Server
{
	[Property, ReadOnly]
	[Feature( Library.AGENT ), Group( DEBUG )]
	public List<BasePawn> AllPawns => BasePawn.GetAll<BasePawn>().ToList();

	[Property, ReadOnly]
	[Feature( Library.AGENT ), Group( DEBUG )]
	public List<BasePawn> ActivePawns => BasePawn.GetAllActive<BasePawn>().ToList();

	/// <summary>
	/// Tries to get a <see cref="Client"/> from its connection.
	/// </summary>
	/// <returns> The <see cref="Client"/>(or null). </returns>
	public static Client FindClient( Connection cn )
	{
		if ( cn is null )
			return null;

		return ValidClients.FirstOrDefault( cl => cl.CompareConnection( cn ) );
	}

	/// <summary>
	/// Tries to find the <see cref="Client"/> from its connection and get its pawn.
	/// </summary>
	public static BasePawn FindPawn( Connection cn )
		=> (FindClient( cn )?.Pawn is BasePawn pawn && pawn.IsValid()) ? pawn : null;

	/// <summary>
	/// Tries to find the <see cref="Client"/> from its connection and find its <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"> ♟ </typeparam>
	/// <returns> The <typeparamref name="T"/>(or null). </returns>
	public static T FindPawn<T>( Connection cn ) where T : BasePawn
		=> FindClient( cn )?.Pawn as T;

	/// <summary>
	/// Tries to find the <see cref="Client"/> from its connection and get its pawn.
	/// </summary>
	/// <returns> The pawn(or null). </returns>
	public static bool TryFindPawn( Connection cn, out BasePawn pawn )
		=> (pawn = FindPawn( cn )).IsValid();

	/// <summary>
	/// Tries to find the <see cref="Client"/> from its connection and find its <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"> ♟ </typeparam>
	/// <returns> The <typeparamref name="T"/>(or null). </returns>
	public static bool TryFindPawn<T>( Connection cn, out T pawn ) where T : BasePawn
		=> (pawn = FindPawn<T>( cn )).IsValid();
}
