using System.Text.Json.Serialization;

namespace GameFish;

/// <summary>
/// Lets you choose from a dictionary with the likelihood being greater the higher the number is.
/// </summary>
public struct WeightedList<T>
{
	[KeyProperty, WideMode]
	public Dictionary<T, float> Weights { get; set; }

	[ReadOnly, JsonIgnore]
	private double? TotalWeight { get; set; }

	[Hide, ReadOnly]
	[Group( BaseEntity.DEBUG )]
	public readonly double? TotalWeightCached => TotalWeight;

	public WeightedList() { }

	public WeightedList( Dictionary<T, float> weights )
	{
		Weights = new( weights ?? [] );
	}

	public WeightedList( in WeightedList<T> wl )
	{
		Weights = new( wl.Weights ?? [] );
	}

	public double GetTotalWeightCached() =>
		TotalWeight ?? CalculateWeight();

	/// <summary>
	/// Forces the weight to be (re)calculated.
	/// </summary>
	/// <returns> The resulting weight(or <c>0</c>). </returns>
	[Button]
	public double CalculateWeight()
	{
		if ( Weights is null )
			return 0d;

		double weight = 0d;

		foreach ( float wgt in Weights.Values )
			weight += wgt.Positive();

		TotalWeight = weight;

		return weight;
	}

	/// <returns> A result weighted by randomness(or <paramref name="default"/>). </returns>
	public T Pick( T @default = default )
		=> Random.TryGetWeighted( Weights, GetTotalWeightCached(), out var result ) ? result : @default;

	/// <summary>
	/// Tries to get a result weighted by randomness.
	/// </summary>
	/// <returns> If a result was found. </returns>
	public bool TryPick( out T result )
		=> Random.TryGetWeighted( Weights, GetTotalWeightCached(), out result );

	/// <summary>
	/// Tries to get a result, removing it from its list if successful.
	/// </summary>
	/// <returns> If a result was found and removed. </returns>
	public bool TryTake( out T result )
	{
		if ( !Random.TryGetWeighted( Weights, GetTotalWeightCached(), out result ) )
			return false;

		return Weights?.Remove( result ) ?? false;
	}
}
