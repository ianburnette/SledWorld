using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(AdditionalGravityConfig), menuName = "ScriptableObjects/" + nameof(AdditionalGravityConfig))]
    public class AdditionalGravityConfig : ScriptableObject { public float gravity; }
}