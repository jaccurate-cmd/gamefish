using System;

namespace GameFish;

/// <summary>
/// Min and max floats without a fixed value setting.
/// </summary>
public struct FloatRange
{
	[KeyProperty] public float Min { get; set; }
	[KeyProperty] public float Max { get; set; }

	public FloatRange() { }
	public FloatRange( float min, float max ) { Min = min; Max = max; }

	public readonly float Lerp( [Range( 0f, 1f )] in float frac, in bool clamp = true )
		=> MathX.Lerp( Min, Max, frac, clamp: clamp );

	public readonly float Remap( in float value, in float from = 0f, in float to = 1f, in bool clamp = true )
		=> MathX.Remap( value, Min, Max, from, to, clamp );

	public readonly int RemapFloor( in float value, in float from = 0f, in float to = 1f, in bool clamp = true )
		=> Remap( value, from, to, clamp ).FloorToInt();

	public readonly int RemapCeil( in float value, in float from = 0f, in float to = 1f, in bool clamp = true )
		=> Remap( value, from, to, clamp ).CeilToInt();

	public readonly float GetRandom()
		=> Random.Float( Min, Max );

	public static implicit operator FloatRange( Range r ) => new( r.Start.Value, r.End.Value );
	public static implicit operator FloatRange( RangedFloat r ) => new( r.Min, r.Max );
}
