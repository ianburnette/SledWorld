using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(GroundStickinessConfig), menuName = "ScriptableObjects/" + nameof(GroundStickinessConfig))]
    public class GroundStickinessConfig : ScriptableObject { public float groundStickForce; }
}