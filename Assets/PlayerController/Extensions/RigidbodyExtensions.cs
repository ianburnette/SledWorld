using UnityEngine;

public static class RigidbodyExtensions
{
    public static void OverrideYVelocity(this Rigidbody rigidbody, float newYVelocity)
    {
        Vector3 velocity;
        velocity = new Vector3((velocity = rigidbody.velocity).x, newYVelocity, velocity.z);
        rigidbody.velocity = velocity;
    }
}