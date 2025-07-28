using System;

namespace GameFish;

/// <summary>
/// Min and max integers without the fixed value setting.
/// </summary>
public struct IntRange
{
	[KeyProperty] public int Min { get; set; }
	[KeyProperty] public int Max { get; set; }

	public IntRange() { }
	public IntRange( int min, int max ) { Min = min; Max = max; }

	public readonly float Lerp( [Range( 0f, 1f )] in float frac, in bool clamp = true )
		=> MathX.Lerp( Min, Max, frac, clamp: clamp );

	public readonly float Remap( in float value, in float from = 0f, in float to = 1f, in bool clamp = true )
		=> MathX.Remap( value, Min, Max, from, to, clamp );

	public readonly int RemapFloor( in float value, in float from = 0f, in float to = 1f, in bool clamp = true )
		=> Remap( value, from, to, clamp ).FloorToInt();

	public readonly int RemapCeil( in float value, in float from = 0f, in float to = 1f, in bool clamp = true )
		=> Remap( value, from, to, clamp ).CeilToInt();

	public readonly int GetRandom()
		=> Random.Int( Min, Max );

	public static implicit operator IntRange( Range r ) => new( r.Start.Value, r.End.Value );
	public static implicit operator IntRange( RangedFloat r ) => new( r.Min.FloorToInt(), r.Max.CeilToInt() );
}
