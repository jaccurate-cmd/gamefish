namespace GameFish;

public partial class SpectatorPawn
{
	private const string GROUP_FREE_ROAM = "Free Roam";

	[Property]
	[FeatureEnabled( GROUP_FREE_ROAM )]
	protected bool HasFreeRoamMode { get; set; }

	[Property]
	[Feature( GROUP_FREE_ROAM )]
	[Range(1, 1000)]
	public float Speed { get; set; } = 10f;

	[Property]
	[Feature( GROUP_FREE_ROAM )]
	[Range(1, 1000)]
	public float? SprintSpeed { get; set; } = 20f;

	/// <summary>
	/// Should the free roam controller limit itself to the <see cref="Bounds"/>.
	/// </summary>
	[Property]
	[Feature( GROUP_FREE_ROAM )]
	public bool HasBounds { get; set; } = false;
	
	/// <summary>
	/// Hard bounds for the free roam mode. Uses global transform.
	/// </summary>
	[Property]
	[Feature(GROUP_FREE_ROAM)]
	[ShowIf(nameof(HasBounds), true)]
	public BBox Bounds { get; set; }
}
