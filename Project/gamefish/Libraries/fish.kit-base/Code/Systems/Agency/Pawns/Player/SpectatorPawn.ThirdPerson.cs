namespace GameFish;

public partial class SpectatorPawn
{
	private const string GROUP_THIRD_PERSON = "Third-Person";

	[Property]
	[FeatureEnabled( GROUP_THIRD_PERSON )]
	protected bool HasThirdPersonMode { get; set; }

	[Property]
	[Feature( GROUP_THIRD_PERSON )]
	[Range(1, 1000)]
	public float MaxDistance { get; set; } = 200f;

	[Property]
	[Feature( GROUP_THIRD_PERSON )]
	[Range(1, 1000)]
	public float MinDistance { get; set; } = 50f;
}
