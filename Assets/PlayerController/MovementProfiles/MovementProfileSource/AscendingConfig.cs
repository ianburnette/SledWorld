using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(AscendingConfig), menuName = "ScriptableObjects/" + nameof(AscendingConfig))]
    public class AscendingConfig : ScriptableObject { public float jumpReleaseVelocity; }

}