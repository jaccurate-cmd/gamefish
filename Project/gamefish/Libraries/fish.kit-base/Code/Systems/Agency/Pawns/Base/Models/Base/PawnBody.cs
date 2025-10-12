namespace GameFish;

public abstract partial class PawnBody : BaseEntity
{
	public const string GROUP_OPACITY = "Opacity";

	/// <summary>
	/// The speed that this model back fades in on its own.
	/// </summary>
	[Property]
	[Feature( MODEL ), Group( GROUP_OPACITY )]
	public float OpacitySpeed { get; set; } = 2f;

	[Property]
	[Range( 0f, 1f ), Step( 0.001f )]
	[Feature( DEBUG ), Order( DEBUG_ORDER ), Group( GROUP_OPACITY )]
	public virtual float Opacity
	{
		get => _opacity;
		set
		{
			_opacity = value.Clamp( 0f, 1f );
			SetOpacity( _opacity );
		}
	}

	protected float _opacity;

	public virtual Model Model { get => GetModel(); set => SetModel( value ); }

	protected override void OnUpdate()
	{
		UpdateOpacity();
	}

	public abstract Model GetModel();
	public abstract void SetModel( Model mdl );

	public virtual void SetOpacity( in float a )
	{
		Opacity = a;
	}

	protected virtual void UpdateOpacity()
	{
		if ( Opacity >= 1f )
			return;

		Opacity += Time.Delta * OpacitySpeed;
	}

	/// <summary>
	/// Called from a pawn to manage things like distance fading.
	/// </summary>
	public virtual void OnViewUpdate( PawnView view )
	{
		if ( view.IsValid() )
			SetOpacity( OpacityFromDistance( view.DistanceFromEye ) );
	}

	// Hardcoded for consistency but you can easily override this.
	public virtual float OpacityFromDistance( in float distance )
		=> (distance * WorldScale.x.NonZero( 0.1f )).Remap( 15f, 25f );

	/// <summary>
	/// Set an animation parameter on the model rendering component.
	/// </summary>
	public virtual void SetAnim<T>( in string key, in T value )
	{
	}
}
