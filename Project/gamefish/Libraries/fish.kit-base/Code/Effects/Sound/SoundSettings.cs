namespace GameFish;

/// <summary>
/// Where and how the sound should be played.
/// </summary>
public struct SoundSettings
{
	/// <summary>
	/// The object this sound is attached to.
	/// </summary>
	public GameObject Following { get; set; }

	/// <summary>
	/// This is treated as local if <see cref="Following"/> is defined.
	/// </summary>
	public Transform? Transform { get; set; }

	public float? Pitch { get; set; }
	public float? Volume { get; set; }

	public string Mixer { get; set; }

	public SoundSettings() { }

	public SoundSettings( Vector3 worldPos )
	{
		Transform = new( worldPos );
	}

	public SoundSettings( Vector3 localPos, GameObject obj )
	{
		Following = obj;

		if ( obj.IsValid() )
			Transform = new( obj.WorldTransform.PointToLocal( localPos ) );
	}
}
