using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(JumpConfig), menuName = "ScriptableObjects/" + nameof(JumpConfig))]
    public class JumpConfig : ScriptableObject { public float jumpForce; }
}