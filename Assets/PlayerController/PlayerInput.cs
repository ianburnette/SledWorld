using System;
using PlayerController;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    GroundDetection _groundDetection; //TODO: should this be here?
    ObstacleAvoidance _obstacleAvoidance;
    
    [Header("Debug")] 
    [SerializeField] bool drawWorldMovementVector;
    [SerializeField] bool drawSlopeSensitiveWorldMovementVector;
    [SerializeField] bool drawCorrectedMovementVector;

    Vector3 inputRelativeToWorldPlane, inputRelativeToSlope, correctedMovement;
    Camera mainCamera;
    
    public delegate void JumpPressDelegate();
    public static event JumpPressDelegate OnJumpPress;   
    
    public delegate void SledPressDelegate();
    public static event SledPressDelegate OnSledPress;

    public delegate void JumpReleaseDelegate();
    public static event JumpReleaseDelegate OnJumpRelease;

    public delegate void MoveDelegate(Vector3 directionToMoveIn);
    public static event MoveDelegate OnMove;

    void Awake()
    {
        _groundDetection = GetComponent<GroundDetection>();
        _obstacleAvoidance = GetComponent<ObstacleAvoidance>();
    }

    void Start() => mainCamera = Camera.main;

    void Update()
    {
        HandleInput();
        DrawDebug();
    }

    void HandleInput()
    {
        HandleJumpInput();
        HandleSledInput();
        HandleMovementInput();
    }
    
    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump")) OnJumpPress?.Invoke();
        if (Input.GetButtonUp("Jump")) OnJumpRelease?.Invoke();
    }

    void HandleSledInput()
    {
        if (Input.GetButtonDown("Sled")) OnSledPress?.Invoke();
    }

    void HandleMovementInput()
    {
        var normalizedRawInput = 
            new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).NormalizeIfMagnitudeGreaterThanOne();
        var movementVector = CalculateMovementVector(normalizedRawInput);
        OnMove?.Invoke(movementVector);
    }

    Vector3 CalculateMovementVector(Vector2 normalizedRawInput)
    {
        inputRelativeToWorldPlane = MovementUtils.MovementRelativeToCamera(normalizedRawInput, mainCamera);
        inputRelativeToSlope = _groundDetection.InputAdjustedForCurrentSlope(inputRelativeToWorldPlane);
        correctedMovement = _obstacleAvoidance.AvoidObstacles(inputRelativeToSlope,  _groundDetection.GroundRelationship_.groundNormal);
        return correctedMovement;
    }
    
    void DrawDebug()
    {
        var position = transform.position;
        if (drawWorldMovementVector)
            Debug.DrawRay(position, inputRelativeToWorldPlane, Color.green);		
        if (drawSlopeSensitiveWorldMovementVector)
            Debug.DrawRay(position, inputRelativeToSlope, Color.red);
        if (drawCorrectedMovementVector)
            Debug.DrawRay(position, correctedMovement, Color.magenta);
    }
}