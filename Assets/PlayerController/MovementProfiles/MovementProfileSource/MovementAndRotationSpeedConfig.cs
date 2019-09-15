using UnityEngine;

namespace PlayerController
{
    [CreateAssetMenu(fileName = nameof(MovementAndRotationSpeedConfig), menuName = "ScriptableObjects/" + nameof(MovementAndRotationSpeedConfig))]
    public class MovementAndRotationSpeedConfig : ScriptableObject
    {
        public float movementSpeed;
        public float rotationSpeed;
    }
}