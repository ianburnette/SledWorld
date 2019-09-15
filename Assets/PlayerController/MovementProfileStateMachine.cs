using System;
using System.Collections.Generic;
using PlayerController;
using UnityEngine;

public class MovementProfileStateMachine : InputListener
{
    [Header("Profiles - THESE MUST BE MANUALLY ASSIGNED")] 
    [SerializeField] MovementProfile walkMovementProfile;
    [SerializeField] MovementProfile ascendingMovementProfile;
    [SerializeField] MovementProfile descendingMovementProfile;
    [SerializeField] MovementProfile skidMovementProfile;
    [SerializeField] MovementProfile sledGroundedMovementProfile;
    [SerializeField] MovementProfile sledAirborneMovementProfile;

    [Header("Transition Config")] 
    [SerializeField] float pivotTime;
    [SerializeField] float minAngleToTriggerPivot;
    [SerializeField] float pivotMinMagnitude;

    [Header("Class References")] 
    GroundDetection _groundDetection;
    PlayerMove _playerMove;
    CharacterMotor _characterMotor;
    PlayerJump _playerJump;
    PlayerSled _playerSled;

    [Header("Debug")]
    [ReadOnly] [SerializeField] MovementProfile currentProfile;
    [ReadOnly] [SerializeField] float timeAtWhichSkidBegan;

    Vector3 _currentMovementVector;

    Dictionary<MovementProfile, Action> _profileEntranceActions;

    MovementProfile CurrentProfile
    {
        get => currentProfile;
        set
        {
            if (currentProfile == value) return;
            currentProfile = value;
            if (_profileEntranceActions.ContainsKey(currentProfile))
                _profileEntranceActions[currentProfile]();
        }
    }

    protected override void SubscribeToInputListeners() => PlayerInput.OnMove += UpdateCurrentMovementVector;
    protected override void UnsubscribeFromInputListeners() => PlayerInput.OnMove -= UpdateCurrentMovementVector;

    void Start()
    {
        _groundDetection = GetComponent<GroundDetection>();
        _playerMove = GetComponent<PlayerMove>();
        _characterMotor = GetComponent<CharacterMotor>();
        _playerJump = GetComponent<PlayerJump>();
        _playerSled = GetComponent<PlayerSled>();
        SetUpProfileEntranceActions();
    }

    void SetUpProfileEntranceActions()
    {
        _profileEntranceActions = new Dictionary<MovementProfile, Action>()
        {
            {skidMovementProfile, BeginSkid}
        };
    }

    void UpdateCurrentMovementVector(Vector3 directionToMoveIn) => _currentMovementVector = directionToMoveIn;
    
    void Update()
    {
        CurrentProfile = SelectMovementProfile();
        _playerMove.CurrentProfile = CurrentProfile;
        _playerJump.CurrentProfile = CurrentProfile;
    }

    MovementProfile SelectMovementProfile()
    {
        return OnSled() ? SelectOnSledMovementProfile() : SelectOnFootMovementProfile();
    }

    MovementProfile SelectOnSledMovementProfile() => 
        Grounded() ? sledGroundedMovementProfile : sledAirborneMovementProfile;

    MovementProfile SelectOnFootMovementProfile()
    {
        if (!Grounded()) return Ascending() ? ascendingMovementProfile : descendingMovementProfile;
        return ShouldBeSkidding() ? skidMovementProfile : walkMovementProfile;
    }

    bool OnSled() => _playerSled.OnSled;

    bool ShouldBeSkidding() => ShouldBeginSkid() || ShouldContinueSkidding();

    bool ShouldBeginSkid() => 
        DifferenceBetweenIntendedMovementAndVelocity() > minAngleToTriggerPivot && 
        _characterMotor.PlanarVelocity().magnitude > pivotMinMagnitude && 
        CurrentProfile != skidMovementProfile;

    float DifferenceBetweenIntendedMovementAndVelocity() => 
        Mathf.Abs(Vector3.Angle(_currentMovementVector, _characterMotor.PlanarVelocity()));

    void BeginSkid() => timeAtWhichSkidBegan = Time.time;
    
    bool ShouldContinueSkidding() => Time.time - timeAtWhichSkidBegan < pivotTime;

    bool Grounded() => _groundDetection.GroundRelationship_.grounded;

    bool Ascending() => _characterMotor.CurrentVelocity.y > 0;
}