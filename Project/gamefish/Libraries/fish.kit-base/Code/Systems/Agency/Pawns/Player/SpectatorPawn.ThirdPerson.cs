using System;

namespace GameFish;

public partial class SpectatorPawn
{
	protected const string GROUP_THIRD_PERSON = "Third Person";

	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode), Label = GROUP_THIRD_PERSON )]
	protected bool HasThirdPersonMode { get; set; }

	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode) )]
	public FloatRange CameraDistance { get; set; } = new(50f, 500f);

	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode) )]
	[Range( 0, 180 )]
	public float PitchClamp { get; set; } = 90f;

	/// <summary>
	/// Sensitivity of the mouse wheel when used to change the distance.
	///
	/// Can be negative to invert the mouse wheel direction, or zero
	/// to disable the mouse wheel altogether.
	/// </summary>
	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode) )]
	public float MouseWheelSensitivity { get; set; } = 5f;

	/// <summary>
	/// A value that is passed to <see cref="MathX.Lerp(float,float,float,bool)"/>.
	///
	/// Can be set to <c>null</c> to disable smoothing altogether 
	/// </summary>
	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode) )]
	public float? MouseWheelSmoothing { get; set; } = 5f;

	/// <summary>
	/// Radius of the sphere collider that will prevent the camera from fazing through walls.
	///
	/// Set to <c>null</c> to disable the camera collisions.  
	/// </summary>
	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode) )]
	public float? CameraColliderRadius { get; set; } = 5f;

	/// <summary>
	/// Tags of objects that will obstruct the camera view.
	/// </summary>
	[Property]
	[Feature( FEATURE_SPECTATOR )]
	[ToggleGroup( nameof(HasThirdPersonMode) )]
	public TagSet CameraColliderTags { get; set; } = new();

	public float CurrentDistance { get; set; }
	public float DesiredDistance { get; set; }
	public Angles CurrentRotation { get; set; }

	protected void ThirdPersonUpdate( float deltaTime )
	{
		if ( !Target.IsValid() ) return;

		if ( Scene?.Camera is not { } camera ) return;

		var angles = (CurrentRotation + Input.AnalogLook) with { roll = 0.0f };
		if ( PitchClamp > 0f )
			angles.pitch = angles.pitch.Clamp( -PitchClamp, PitchClamp );
		CurrentRotation = angles;

		DesiredDistance -= Input.MouseWheel.y * MouseWheelSensitivity * deltaTime;
		if ( CameraColliderRadius is { } radius )
		{
			var trace = Scene.Trace.Sphere( radius,
					Target.EyePosition,
					Target.EyePosition - CurrentRotation.Forward * DesiredDistance )
				.IgnoreGameObject( Target.GameObject )
				.WithAnyTags( CameraColliderTags )
				.Run();

			// DebugOverlay.ScreenText( Vector2.One * 20,
			// 	$"{DesiredDistance} {trace.Distance} {trace.Hit} {trace.GameObject?.Name}",
			// 	flags: TextFlag.LeftTop );
			// DebugOverlay.Line( trace.StartPosition, trace.EndPosition, Color.Red, overlay: true );
			DesiredDistance = trace.Distance;
			// When obstructed, force the current distance to a safe distance that does not clip through the ground.
			if ( trace.Hit )
				CurrentDistance = MathF.Min( CurrentDistance, trace.Distance );
		}

		DesiredDistance = DesiredDistance.Clamp( CameraDistance );

		if ( MouseWheelSmoothing is { } mws )
			CurrentDistance = CurrentDistance.LerpTo( DesiredDistance, mws * deltaTime );

		// Also clamp the current distance, because it starts with 0
		CurrentDistance = CurrentDistance.Clamp( CameraDistance );

		camera.WorldPosition = Target.EyePosition - CurrentRotation.Forward * CurrentDistance;
		camera.WorldRotation = CurrentRotation;
	}
}
