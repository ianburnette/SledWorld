using System;
using UnityEngine;

[Serializable]
public class GroundRelationship
{
    public bool grounded;
    public Vector3 groundNormal;
    public float groundAngle;

    public void Set(bool grounded, Vector3 groundNormal = default, float groundAngle = 0)
    {
        this.grounded = grounded;
        this.groundNormal = groundNormal;
        this.groundAngle = groundAngle;
    }
}