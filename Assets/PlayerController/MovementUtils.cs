using UnityEngine;

public static class MovementUtils
{
   
    public static Vector3 MovementRelativeToCamera(Vector2 input, Camera cam) {
        var screenMovementSpace = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        var screenMovementForward = screenMovementSpace * Vector3.forward;
        var screenMovementRight = screenMovementSpace * Vector3.right;
        return (screenMovementForward * input.y) + (screenMovementRight * input.x);
    }
}