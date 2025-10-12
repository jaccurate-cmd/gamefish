namespace GameFish;

partial class Random
{
	public static double GetTotalWeight<T>( Dictionary<T, float> dict )
	{
		double weight = 0d;

		foreach ( float wgt in dict.Values )
			weight += wgt.Positive();

		return weight;
	}

	/// <summary>
	/// Pick a random key from a dictionary with the likelihood being increased by its value. <br />
	/// It's probably better that you use <see cref="WeightedList{T}.TryPick(out T)"/> instead.
	/// </summary>
	/// <param name="dict"> The probabilities. </param>
	/// <param name="result"> The random result(or <c>default</c>). </param>
	/// <returns> If a value was able to be retrieved. </returns>
	public static bool TryGetWeighted<T>( Dictionary<T, float> dict, out T result )
		=> TryGetWeighted( dict, GetTotalWeight( dict ), out result );

	/// <summary>
	/// Pick a random key from a dictionary with the likelihood being increased by its value. <br />
	/// This version of the method requires that you have predetermined its maximum weight. <br />
	/// It's probably better that you use <see cref="WeightedList{T}.TryPick(out T)"/> instead.
	/// </summary>
	/// <param name="dict"> The probabilities. </param>
	/// <param name="totalWeight"> The pre-defined total weight. </param>
	/// <param name="result"> The random result(or <c>default</c>). </param>
	/// <returns> If a value was able to be retrieved. </returns>
	public static bool TryGetWeighted<T>( Dictionary<T, float> dict, in double totalWeight, out T result )
	{
		if ( dict is null )
		{
			Print.Warn( $"{typeof( Random )}.{nameof( TryGetWeighted )}", "passed in dictionary was null!" );

			result = default;
			return false;
		}

		var values = dict.Values;
		var valueCount = values.Count;

		double randomValue = totalWeight * Double( 0f, 1f );

		for ( var i = 0; i < valueCount; i++ )
		{
			var chance = values.ElementAtOrDefault( i );
			randomValue -= chance;

			if ( randomValue <= 0 )
			{
				result = dict.Keys.ElementAtOrDefault( i );
				return true;
			}
		}

		result = default;
		return false;
	}
}
