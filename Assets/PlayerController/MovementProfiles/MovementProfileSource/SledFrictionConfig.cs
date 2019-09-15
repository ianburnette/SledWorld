using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(SledFrictionConfig), menuName = "ScriptableObjects/" + nameof(SledFrictionConfig))]
    public class SledFrictionConfig : ScriptableObject { public float slopeDownForce; }
}