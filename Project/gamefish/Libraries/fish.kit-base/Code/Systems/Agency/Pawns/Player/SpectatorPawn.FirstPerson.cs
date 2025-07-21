namespace GameFish;

public partial class SpectatorPawn
{
	private const string GROUP_FIRST_PERSON = "First-Person";
	
	[Property]
	[FeatureEnabled( GROUP_FIRST_PERSON )]
	protected bool HasFirstPersonMode { get; set; }

	[Property]
	[Feature( GROUP_FIRST_PERSON )]
	public bool ShowViewModel { get; set; } = true;

	protected void FirstPersonUpdate()
	{
		if ( !Target.IsValid() ) return;

		if ( Scene?.Camera is not { } camera ) return;
		
		camera.WorldPosition = Target.EyePosition;
		camera.WorldRotation = Target.EyeRotation;
		DebugOverlay.Line( Target.EyePosition, Target.EyeForward * 100 );
	}
}
