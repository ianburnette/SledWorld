using UnityEngine;

public static class Vector2Extensions
{
    public static Vector2 NormalizeIfMagnitudeGreaterThanOne(this Vector2 vector2) =>
        vector2.magnitude > 1f ? vector2.normalized : vector2;
}