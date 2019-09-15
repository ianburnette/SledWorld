using System;
using UnityEngine;

namespace PlayerController
{
    public class ObstacleAvoidance : MonoBehaviour
    {
        [SerializeField] float movementCorrectionCheckDistance = 2f;
        [SerializeField] LayerMask collisionMask;
        [SerializeField] int angleIncrement = 6;
        [SerializeField] int maxAngle = 420;
        [SerializeField] float sphereCastRadius = .5f;
        [SerializeField] float raycastTransformOffset = .45f;
        [SerializeField] float maxObstacleYnormal = .1f;

        //TODO: how to label?
        RaycastHit hit;

        void Awake() => collisionMask = LayerMask.GetMask("Default");

        public Vector3 AvoidObstacles(Vector3 movementToCorrect, Vector3 upAxis)
        {
            var correctedVector = movementToCorrect;
            VectorCollidesWithSomething(movementToCorrect);
            if (hit.transform)
                AttemptToFindBetterPath(ref correctedVector, upAxis);
            return correctedVector;
        }

        void AttemptToFindBetterPath(ref Vector3 currentVector, Vector3 upAxis)
        {
            for (var i = angleIncrement; i < maxAngle / angleIncrement; i += angleIncrement)
            {
                if (WayIsClear(ref currentVector, upAxis, i)) return;
                if (WayIsClear(ref currentVector, upAxis, -i)) return;
            }
        }

        bool WayIsClear(ref Vector3 currentVector, Vector3 upAxis, int angle)
        {
            var testVector = Quaternion.AngleAxis(angle, upAxis) * currentVector;
            if (VectorCollidesWithSomething(testVector)) return false;
            currentVector = testVector;
            return true;
        }

        bool VectorCollidesWithSomething(Vector3 vector)
        {
            var ray = new Ray(transform.position + (Vector3.down * raycastTransformOffset), vector);
            Physics.SphereCast(ray.origin, radius: sphereCastRadius, direction: ray.direction, out hit,
                movementCorrectionCheckDistance, collisionMask);
            Debug.DrawRay(ray.origin, ray.direction * movementCorrectionCheckDistance,
                ShouldAvoidHit() ? Color.cyan : Color.blue);
            return ShouldAvoidHit();
        }

        bool ShouldAvoidHit() => hit.transform && hit.normal.y <= maxObstacleYnormal;
    }
}