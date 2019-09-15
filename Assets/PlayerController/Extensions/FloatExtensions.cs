using UnityEngine;

public static class FloatExtensions
{
    public static float PositiveOrZero(this float myFloat) => Mathf.Abs(myFloat) > 0 ? myFloat : 0;
}
