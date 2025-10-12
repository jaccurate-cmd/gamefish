namespace GameFish;

partial class BaseEquip
{
	/// <summary> Should an NPC consider this for combat? </summary>
	[Property]
	[Feature( NPC ), Group( COMBAT )]
	public virtual bool UseInCombat { get; set; } = true;

	/// <summary> Should an NPC attack with this weapon on their own? </summary>
	[Property]
	[Feature( NPC ), Group( COMBAT )]
	public virtual bool AllowAutoAttack { get; set; } = true;

	/// <summary>
	/// An NPC shouldn't use this outside of these ranges.
	/// They'll attack in this range but try to get within <see cref="IdealRange"/> still.
	/// </summary>
	[Property, Feature( NPC ), Group( RANGE )]
	public virtual FloatRange UsableRange { get; set; } = new( 0f, 2048f );

	/// <summary>
	/// An NPC will try to get within this range when using this.
	/// If it is not encompassed by <see cref="UsableRange"/> then bugs may happen.
	/// </summary>
	[Property, Feature( NPC ), Group( RANGE )]
	public virtual FloatRange IdealRange { get; set; } = new( 200f, 1000f );

	/// <returns> If an NPC will ever bother with this equipment. </returns>
	public virtual bool IsUsable( BaseNPC npc, in bool forCombat )
		=> forCombat == UseInCombat;

	/// <returns> If an NPC is allowed to use this at the specified distance. </returns>
	public virtual bool UsableAtDistance( float targetDist )
		=> UsableRange.Within( targetDist );

	/// <returns> If this is an ideal position. </returns>
	public virtual float GetIdealDistance( float targetDist )
		=> targetDist.Clamp( IdealRange );

	/// <summary>
	/// Allows you to make this weapon selected over others
	/// with respect to distance from the target.
	/// </summary>
	/// <returns> A number that's lower the better it is for this distance. </returns>
	public virtual float GetSelectionPriority( float targetDist )
		=> (targetDist - IdealRange.Min).Abs();

	/// <summary>
	/// Overrides where an NPC aims this(if not null).
	/// Would be very useful for aiming projectiles ahead.
	/// </summary>
	/// <param name="pawn"> The shmuck. </param>
	/// <param name="baseAim"> Where the NPC would aim by default(or not). </param>
	public virtual Vector3? GetTargetAimPosition( BasePawn pawn, in Vector3? baseAim = null )
		=> null;
}
