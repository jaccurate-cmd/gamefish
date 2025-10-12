namespace GameFish;

partial class BasePawn : ISimulate
{
	// TODO: Move spec refs to this in a new controller.
	public virtual Vector3 WishVelocity
	{
		get => _wishVelocity;
		set => _wishVelocity = value;
	}

	protected Vector3 _wishVelocity;

	/// <returns> If this pawn should listen to the local client's input. </returns>
	public virtual bool AllowInput()
	{
		if ( !Agent.IsValid() )
			return false;

		if ( Agent.IsPlayer )
			return Agent.IsOwner();

		return false;
	}

	public virtual bool CanSimulate()
		=> AllowInput();

	public virtual void FrameSimulate( in float deltaTime )
	{
		UpdateController( in deltaTime, isFixedUpdate: false );
		SimulateView( in deltaTime );
	}

	public virtual void FixedSimulate( in float deltaTime )
	{
	}
}
