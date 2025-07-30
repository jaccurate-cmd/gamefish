namespace GameFish;

partial class PawnView
{
	public bool HasFirstPersonMode => IsModeAllowed( Perspective.FirstPerson );

	[Property]
	[Feature( MODES )]
	[Group( FIRST_PERSON ), Order( FIRST_PERSON_ORDER )]
	public bool ShowViewModel { get; set; } = true;

	protected virtual void OnFirstPersonModeSet()
	{
	}

	protected virtual void SetFirstPersonModeTransform()
	{
		SetTransformFromRelative();
	}

	protected virtual void OnFirstPersonModeUpdate( in float deltaTime )
	{
		var pawn = TargetPawn;

		if ( !pawn.IsValid() )
			return;

		Relative = new();

		UpdateViewModel( in deltaTime );
	}

	protected virtual void ToggleViewModel( bool isEnabled )
	{
		var vm = ViewModel;

		if ( vm.IsValid() )
			vm.GameObject.Enabled = isEnabled;
	}

	protected virtual void UpdateViewModel( in float deltaTime )
	{
		if ( Mode != Perspective.FirstPerson )
			return;

		var vm = ViewModel;

		if ( !vm.IsValid() )
			return;

		var isFirstPerson = IsFirstPerson();

		ToggleViewModel( isFirstPerson );

		if ( isFirstPerson )
			vm.UpdateOffset( deltaTime );
	}

	/// <summary>
	/// Determines if we're actually in first person based on
	/// the distance from this view to the eye position.
	/// </summary>
	protected virtual bool IsFirstPerson()
	{
		if ( Mode != Perspective.FirstPerson )
			return false;

		return DistanceFromEye <= 5f;
	}
}
