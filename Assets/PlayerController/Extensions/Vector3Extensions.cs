using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 DiscardNegativeValues(this Vector3 myVector3) => 
        new Vector3(myVector3.x.PositiveOrZero(), myVector3.y.PositiveOrZero(), myVector3.z.PositiveOrZero());
}
