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
			PerformTransitionActions();
		}
	}
	public float TimeAtWhichSkidBegan => _timeAtWhichSkidBegan;

	[Header("Slope Behavior")]
	[SerializeField] float maxWalkableSlopeAngle = 80f;
	[SerializeField] float slopeCorrectionForce = 10f;
	public float MaxWalkableSlopeAngle { get => maxWalkableSlopeAngle; set => maxWalkableSlopeAngle = value; }
	
	[Header("Private Variables")]
	GroundDetection groundDetection;
	CharacterMotor characterMotor;
	float _timeAtWhichSkidBegan;
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
		characterMotor = GetComponent<CharacterMotor>();
		groundDetection = GetComponent<GroundDetection>();
	}

	bool Grounded() => groundDetection.GroundRelationship_.grounded;
	void FixedUpdate() {
		UpdatePlayerMovement();
	}

	void UpdatePlayerMovement() => ApplyMovementToMotor();

	void ApplyMovementToMotor()
	{
		if (!currentProfile) return;
		characterMotor.MoveTo(
			transform.position + _currentMovementDirection, 
			currentProfile.movementAndRotationSpeedConfig.movementSpeed, 
			ignoreY: false);

		if (Grounded())
		{
			if (currentProfile.groundStickinessConfig)
				characterMotor.ApplyAdditionalForce(GroundStickinessForce());
			characterMotor.ApplyAdditionalForce(SlopeUp());
		}

		characterMotor.RotateToVelocity(currentProfile.movementAndRotationSpeedConfig.rotationSpeed, true);
		characterMotor.LimitSpeed(false);
	}

	
	Vector3 GroundStickinessForce() => 
		-groundDetection.GroundRelationship_.groundNormal * currentProfile.groundStickinessConfig.groundStickForce;

	void PerformTransitionActions()
	{
		playerCollider.material = currentProfile.colliderMaterial;
		//TODO: handle state exiting, but not here!
		//switch (currentProfile.movementProfile.movement)
		//{
		//	case Movements.Walking:
		//		break;
		//	case Movements.InAir:
		//		break;
		//	case Movements.Skidding:
		//		timeAtWhichSkidBegan = Time.time;
		//		break;
		//	default:
		//		throw new ArgumentOutOfRangeException();
		//}
	}
	
	bool IsOnSteepSlope() => groundDetection.GroundRelationship_.groundAngle > MaxWalkableSlopeAngle;
	Vector3 SlopeUp() => groundDetection.SlopeCorrection() * -Physics.gravity.magnitude;

}
