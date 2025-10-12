namespace GameFish;

partial class BaseController : ISimulate
{
	[Sync]
	public Vector3 WishVelocity { get; set; }

	public bool CanSimulate() => !IsProxy;

	public abstract void Move( in float deltaTime );

	protected virtual void PreMove( in float deltaTime ) { }

	protected virtual void PostMove( in float deltaTime ) { }
}
