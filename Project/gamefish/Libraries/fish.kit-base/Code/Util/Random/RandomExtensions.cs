namespace GameFish;

public static partial class RandomExtensions
{
	/// <returns> Integer between 0 and <paramref name="n"/>. </returns>
	public static int Random( this int n )
		=> n switch
		{
			0 => 0,
			< 0 => global::GameFish.Random.Int( n, 0 ),
			_ => global::GameFish.Random.Int( 0, n )
		};

	/// <returns> Float between 0 and <paramref name="n"/>. </returns>
	public static float Random( this float n )
		=> n switch
		{
			0f => 0f,
			< 0f => global::GameFish.Random.Float( n, 0f ),
			_ => global::GameFish.Random.Float( 0f, n )
		};

	/// <summary>
	/// A random value from any kind of list(or <paramref name="default"/>).
	/// </summary>
	/// <returns> An existing <typeparamref name="T"/>(or <paramref name="default"/>). </returns>
	public static T PickRandom<T>( this IEnumerable<T> list, T @default = default )
		=> global::GameFish.Random.From( list, @default );

	/// <summary>
	/// A random value from an <see cref="IList{T}"/>(or <paramref name="default"/>).
	/// </summary>
	/// <returns> An existing <typeparamref name="T"/>(or <paramref name="default"/>). </returns>
	public static T TakeRandom<T>( this IList<T> list, T @default = default )
		=> global::GameFish.Random.TryTake( list, out T value ) ? value : @default;

	/// <summary>
	/// Tries to pick and then remove a random <typeparamref name="T"/> from an <see cref="IList{T}"/>.
	/// </summary>
	/// <returns> If a <typeparamref name="T"/> was found and thus removed. </returns>
	public static bool TryTakeRandom<T>( this IList<T> list, out T value )
		=> global::GameFish.Random.TryTake( list, out value );
}
