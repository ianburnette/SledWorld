using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(MovementProfile), menuName = "ScriptableObjects/" + nameof(MovementProfile))]
    public class MovementProfile : ScriptableObject
    {
        public PhysicMaterial colliderMaterial;
        public MovementAndRotationSpeedConfig movementAndRotationSpeedConfig;
        public AscendingConfig ascendingConfig;
        public AdditionalGravityConfig additionalGravityConfig;
        public JumpConfig jumpConfig;
        public GroundStickinessConfig groundStickinessConfig;
    }
}