using System;
using System.Linq;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    const int NumOfFloorCheckers = 9;

    [Header("Config")] 
    [SerializeField] float groundCheckRadius = .5f;
    [SerializeField] float groundCheckDistance = 1f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] bool shouldDrawDebug = true;

    [Header("Debug")]
    [ReadOnly] [SerializeField] GroundRelationship groundRelationship = new GroundRelationship();
    public GroundRelationship GroundRelationship_ { get => groundRelationship; set => groundRelationship = value; }
    GroundHitInfo[] _groundInfo;
    
    static readonly Vector3[] FloorCheckMatrix = {
        new Vector3(1, 0, 1),  new Vector3(0, 0, 1),  new Vector3(-1, 0, 1),
        new Vector3(1, 0, 0),  new Vector3(0, 0, 0),  new Vector3(-1, 0, 0),
        new Vector3(1, 0, -1), new Vector3(0, 0, -1), new Vector3(-1, 0, -1),
    };
    
    void Awake()
    {
        groundMask = LayerMask.GetMask("Default");
        groundRelationship = new GroundRelationship();
    }

    void OnEnable() => _groundInfo = new GroundHitInfo[NumOfFloorCheckers];

    void FixedUpdate() => DetectGround(groundMask);

    public void DetectGround(LayerMask groundMask)
    {
        GetGroundInfo(groundMask);
        DetermineGroundRelationship();
    }
    
    void GetGroundInfo(LayerMask groundMask)
    {
        for (var i = 0; i < NumOfFloorCheckers; i++)
            _groundInfo[i] = GetGroundHitInfo(FloorCheckPosition(i), groundMask);
    }

    GroundHitInfo GetGroundHitInfo(Vector3 checker, LayerMask groundMask) {
        //Debug.DrawRay(checker, Vector3.down * distanceToCheck, Color.red);
        return GroundCheckRaycast(checker, groundCheckDistance, groundMask, out var hit)
            ? new GroundHitInfo(hit.point, hit.normal) : null;
    }
    
    static bool GroundCheckRaycast(Vector3 checker, float distanceToCheck, LayerMask groundMask, out RaycastHit hit) => 
        Physics.Raycast(checker, Vector3.down, out hit, distanceToCheck, groundMask);
    
    void DetermineGroundRelationship()
    {
        var groundNormal = AveragedGroundNormal();
        groundRelationship.Set(
            grounded: PointsOfContact() != 0,
            groundNormal: groundNormal,
            groundAngle: Vector3.Angle(groundNormal, Vector3.up)
        );
    }

    Vector3 AveragedGroundNormal()
    {
        var pointsOfContact = PointsOfContact();    
        if (pointsOfContact == 0) return default;
        var summedNormal = _groundInfo.Where(t => t != null).Aggregate(Vector3.zero, (current, t) => current + t.normal);
        return CalculateSlopeNormal(summedNormal, pointsOfContact);
    }
    
    static Vector3 CalculateSlopeNormal(Vector3 summedNormal, int pointsOfContact) =>
        summedNormal.DiscardNegativeValues() / pointsOfContact;

    int PointsOfContact() => _groundInfo.Count(t => t != null);
    Vector3 FloorCheckPosition(int index) => transform.position + FloorCheckMatrix[index] * groundCheckRadius;
    
    public Vector3 SlopeCorrection() => Vector3.Cross(groundRelationship.groundNormal, SlopeTangent());
    Vector3 SlopeTangent() => new Vector3(-groundRelationship.groundNormal.z, 0, groundRelationship.groundNormal.x);

    public Vector3 InputAdjustedForCurrentSlope(Vector3 input) => 
        Vector3.ProjectOnPlane(input, groundRelationship.groundNormal);

    public bool CurrentlyOnValidGround(float maxWalkableSlopeAngle) =>
        groundRelationship.grounded && groundRelationship.groundAngle <= maxWalkableSlopeAngle;

    
    void Update() => AttemptToDrawDebug();

    void AttemptToDrawDebug()
    {
        if (!shouldDrawDebug) return;
        for (var i = 0; i < NumOfFloorCheckers; i++)
        {
            var hittingGround = _groundInfo[i] == null;
            DebugRay(FloorCheckPosition(i),
                hittingGround
                    ? groundCheckDistance
                    : Vector3.Distance(FloorCheckPosition(i), _groundInfo[i].position),
                hittingGround ? Color.red : Color.yellow);
        }
    }

    void DebugRay(Vector3 origin, float verticalMultiplier, Color color) => 
        Debug.DrawRay(origin, Vector3.down * verticalMultiplier, color);
}
