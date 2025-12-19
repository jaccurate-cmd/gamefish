namespace GameFish;

/// <summary>
/// Allows you to block input(where this is checked) from anywhere in the scene.
/// </summary>
public partial interface IFocus
{
	/// <summary>
	/// If true: prevents looking around. <br />
	/// </summary>
	public bool HasAimingFocus { get; }

	/// <summary>
	/// If true: prevents analogue(such as WASD) movement. <br />
	/// </summary>
	public bool HasMovingFocus { get; }

	/// <summary>
	/// If true: prevents jumping, crouching, shooting etc. <br />
	/// </summary>
	public bool HasActionFocus { get; }

	/// <summary>
	/// All focus interfaces in the main, actively played game scene.
	/// </summary>
	public static IEnumerable<IFocus> All => Game.ActiveScene?.GetAll<IFocus>();

	public static bool Aiming => All?.Any( f => f?.HasAimingFocus is true ) is true;
	public static bool Moving => All?.Any( f => f?.HasMovingFocus is true ) is true;
	public static bool Action => All?.Any( f => f?.HasActionFocus is true ) is true;
}
