namespace GameFish;

partial class BasePawn
{
	[Property]
	[Feature( PAWN )]
	public virtual BaseController Controller { get; set; }

	[Property]
	[Feature( PAWN ), Group( MODEL )]
	public virtual PawnBody BodyComponent { get; set; }

	protected virtual void UpdateController( in float deltaTime, in bool isFixedUpdate )
		=> Controller?.FrameSimulate( in deltaTime );

	public override bool TryTeleport( in Transform tDest )
	{
		WorldPosition = tDest.Position;

		// Don't rotate the object itself.
		EyeRotation = tDest.Rotation;

		return true;
	}
}
