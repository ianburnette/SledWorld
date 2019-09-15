using PlayerController;
using UnityEngine;

public class PlayerMove : InputListener {

	[Header("Debug")]
	[ReadOnly] [SerializeField] MovementProfile currentProfile;
	[ReadOnly] [SerializeField] Collider playerCollider;
	public MovementProfile CurrentProfile
	{
		set {
			if (currentProfile == value) return;
			currentProfile = value;
			SetPlayerColliderMaterial();
		}
	}

	[Header("Slope Behavior")]
	[SerializeField] float maxWalkableSlopeAngle = 80f;
	public float MaxWalkableSlopeAngle { get => maxWalkableSlopeAngle; set => maxWalkableSlopeAngle = value; }
	
	[Header("Private Variables")]
	GroundDetection _groundDetection;
	CharacterMotor _characterMotor;
	PlayerSled _playerSled;
	float _previousMovementAngle;
	bool _pivoting;
	Vector2 _currentInputVector;
	Vector3 _movementDirectionRelativeToCamera, _currentMovementDirection;
	
	protected override void SubscribeToInputListeners() => PlayerInput.OnMove += SetCurrentMovementVector;
	protected override void UnsubscribeFromInputListeners() => PlayerInput.OnMove -= SetCurrentMovementVector;

	void SetCurrentMovementVector(Vector3 movementDirection) => _currentMovementDirection = movementDirection;

	void Awake()
	{
		playerCollider = GetComponent<Collider>();
		_characterMotor = GetComponent<CharacterMotor>();
		_groundDetection = GetComponent<GroundDetection>();
		_playerSled = GetComponent<PlayerSled>();
	}

	bool Grounded() => _groundDetection.GroundRelationship_.grounded;

	bool OnSled() => _playerSled.OnSled;
	
	void FixedUpdate() => UpdatePlayerMovement();

	void UpdatePlayerMovement() => ApplyMovementToMotor();

	void ApplyMovementToMotor()
	{
		if (!currentProfile) return;
		
		ApplyVelocityChange();
		ApplyAcceleration();

		_characterMotor.RotateToVelocity(currentProfile.movementAndRotationSpeedConfig.rotationSpeed, true);
		_characterMotor.LimitSpeed(false);

		void ApplyVelocityChange()
		{
			if (OnSled()) return;
			_characterMotor.MoveTo(
				MovementDirection(), currentProfile.movementAndRotationSpeedConfig.movementSpeed, ignoreY: false);
		}
		
		void ApplyAcceleration()
		{
			if (!Grounded()) return;
			if (currentProfile.groundStickinessConfig)
				_characterMotor.ApplyAdditionalForce(GroundStickinessForce());
			if (OnSled())
				_characterMotor.ApplyAdditionalForce(
					MovementDirection() * currentProfile.movementAndRotationSpeedConfig.movementSpeed
					);
			_characterMotor.ApplyAdditionalForce(OnSled() ? SlopeDown() : SlopeUp());
		}
		
		Vector3 MovementDirection() => transform.position + _currentMovementDirection;
	}
	
	Vector3 GroundStickinessForce() => 
		-_groundDetection.GroundRelationship_.groundNormal * currentProfile.groundStickinessConfig.groundStickForce;

	void SetPlayerColliderMaterial() => playerCollider.material = currentProfile.colliderMaterial;

	bool IsOnSteepSlope() => _groundDetection.GroundRelationship_.groundAngle > MaxWalkableSlopeAngle;
	Vector3 SlopeUp() => _groundDetection.SlopeCorrection() * -Physics.gravity.magnitude;

	Vector3 SlopeDown() =>
		_groundDetection.SlopeCorrection() *
		(currentProfile.sledFrictionConfig ? currentProfile.sledFrictionConfig.slopeDownForce : 0f);
}
