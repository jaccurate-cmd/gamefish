namespace GameFish;

/// <summary>
/// Allows you to and manage forces easily on whatever.
/// </summary>
public interface IVelocity
{
    public Vector3 Velocity { get; set; }

    /// <summary>
    /// Tries to modify the velocity. Lets the object modify the result.
    /// </summary>
    /// <returns> If this object allows adding of the velocity. </returns>
    public bool TryModifyVelocity( in Vector3 vel, out Vector3 result )
    {
        if ( !ITransform.IsValid( vel ) )
        {
            result = vel;
            return false;
        }

        Velocity = vel;
        result = vel;

        return true;
    }

    /// <summary>
    /// Tries to modify the velocity. Lets the object modify the result.
    /// </summary>
    /// <returns> If this object allows adding of the velocity. </returns>
    public bool TryModifyVelocity( in Vector3 vel )
        => TryModifyVelocity( vel, out _ );

    /// <summary>
    /// Put your movement/physics code here.
    /// </summary>
    public void ApplyVelocity( in float deltaTime )
    {
    }
}
