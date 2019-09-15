using PlayerController;
using UnityEngine;

public class MovementProfileStateMachine : InputListener
{
    [Header("Profiles - THESE MUST BE MANUALLY ASSIGNED")] 
    [SerializeField] MovementProfile walkMovementProfile;
    [SerializeField] MovementProfile ascendingMovementProfile;
    [SerializeField] MovementProfile descendingMovementProfile;
    [SerializeField] MovementProfile skidMovementProfile;

    [Header("Transition Config")] 
    [SerializeField] float pivotTime;
    [SerializeField] float minAngleToTriggerPivot;
    [SerializeField] float pivotMinMagnitude;

    [Header("Class References")] 
    GroundDetection _groundDetection;
    PlayerMove _playerMove;
    CharacterMotor _characterMotor;
    PlayerJump _playerJump;

    [Header("Globals")]
    Vector3 _currentMovementVector;
    MovementProfile _currentProfile;
    
    protected override void SubscribeToInputListeners() => PlayerInput.OnMove += UpdateCurrentMovementVector;
    protected override void UnsubscribeFromInputListeners() => PlayerInput.OnMove -= UpdateCurrentMovementVector;

    void Awake()
    {
        _groundDetection = GetComponent<GroundDetection>();
        _playerMove = GetComponent<PlayerMove>();
        _characterMotor = GetComponent<CharacterMotor>();
        _playerJump = GetComponent<PlayerJump>();
    }
    
    void UpdateCurrentMovementVector(Vector3 directionToMoveIn) => _currentMovementVector = directionToMoveIn;
    
    void Update()
    {
        _currentProfile = SelectMovementProfile();
        _playerMove.CurrentProfile = _currentProfile;
        _playerJump.CurrentProfile = _currentProfile;
    }

    MovementProfile SelectMovementProfile()
    {
        if (!Grounded()) return Ascending() ? ascendingMovementProfile : descendingMovementProfile;
        return ShouldBeSkidding() ? skidMovementProfile : walkMovementProfile;
    }

    bool ShouldBeSkidding() => ShouldBeginSkid() || ShouldContinueSkidding();

    bool ShouldBeginSkid() =>
        DifferenceBetweenIntendedMovementAndVelocity() > minAngleToTriggerPivot 
        && _characterMotor.PlanarVelocity().magnitude > pivotMinMagnitude;
	
    float DifferenceBetweenIntendedMovementAndVelocity() => 
        Mathf.Abs(Vector3.Angle(_currentMovementVector, _characterMotor.PlanarVelocity()));

    bool ShouldContinueSkidding() => Time.time - _playerMove.TimeAtWhichSkidBegan < pivotTime;

    bool Grounded() => _groundDetection.GroundRelationship_.grounded;

    bool Ascending() => _characterMotor.CurrentVelocity.y > 0;
}
