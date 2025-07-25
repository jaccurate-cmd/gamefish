namespace GameFish;

public partial class SpectatorPawn : PlayerPawn
{
	[Sync]
	[Property]
	[Feature( SPECTATING ), Group( DEBUG )]
	public BasePawn Spectating
	{
		get => _spectating;

		protected set
		{
			_spectating = value;

			if ( this.IsValid() && !IsProxy )
				OnSpectatingSet( value );
		}
	}

	protected BasePawn _spectating;

	/// <summary> Spectators can never be spectated. </summary>
	public override bool AllowSpectators => false;

	/// <summary> Spectators can never be spectated. </summary>
	public override bool CanSpectate( BasePawn spec )
		=> false;

	/// <summary>
	/// Called whenever <see cref="Spectating"/> has been set.
	/// </summary>
	protected virtual void OnSpectatingSet( BasePawn target )
	{
		if ( !target.IsValid() && View.IsValid() )
		{
			WorldPosition = View.WorldPosition;
			WorldRotation = View.WorldRotation;

			View.Mode = PawnView.Perspective.FirstPerson;
		}
	}

	public override bool TrySpectate( BasePawn target )
	{
		if ( !target.IsValid() || !target.CanSpectate( this ) )
			return false;

		Spectating = target;

		return true;
	}

	public override void StopSpectating()
	{
		base.StopSpectating();

		Spectating = null;
	}
}
