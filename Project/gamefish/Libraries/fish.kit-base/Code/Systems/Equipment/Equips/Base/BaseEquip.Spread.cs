namespace GameFish;

partial class BaseEquip : Component.INetworkSpawn
{
	public virtual Rotation GetSpreadConeRotation( float spread )
	{
		if ( spread == 0f )
			return Rotation.Identity;

		return Rotation.Identity
			.RotateAroundAxis( Vector3.Forward, Random.Float( 0f, 360f ) )
			.RotateAroundAxis( Vector3.Right, Random.Float( 0f, spread ) );
	}
}
