using UnityEngine;

public class GroundHitInfo {
    public Vector3 position;
    public Vector3 normal;

    public GroundHitInfo(Vector3 pos, Vector3 norm) {
        position = pos;
        normal = norm;
    }
}