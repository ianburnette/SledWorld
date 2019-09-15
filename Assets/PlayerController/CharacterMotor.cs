using UnityEngine;

public class CharacterMotor : MonoBehaviour, IVelocityLimiter {
	[HideInInspector] public Vector3 currentSpeed;
	[HideInInspector] public float distanceToTarget;
	[SerializeField] float maxMagnitude = 300f;
	
	[Header("References")]
	Rigidbody _myRigidbody;
	
	[Header("Passive Behavior")] 
	public float maxSpeed = 9;
	public float movementSensitivity = .25f;
	public float deceleration = 7.6f; //TODO: remove this? extract to profiles?
	
	Vector3 _targetMovementPosition;
	public Vector3 CurrentVelocity => _myRigidbody.velocity;

	void Awake()
	{
		_myRigidbody = GetComponent<Rigidbody>();
		_myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		_myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	void FixedUpdate()
	{
		_myRigidbody.WakeUp();
		LimitVelocity();
	}

	public void MoveVertical(Vector3 destination) {
		_targetMovementPosition = (destination - transform.position);
		transform.Translate(destination);
	}
	
	public void AddRelativeForce(Vector3 direction, ForceMode forceMode) =>
		_myRigidbody.AddRelativeForce(direction, forceMode);

	public void MoveTo(Vector3 destination, float acceleration, bool ignoreY, ForceMode forceMode = ForceMode.VelocityChange)
	{
		_targetMovementPosition = (destination - transform.position);
		if (ignoreY)
			_targetMovementPosition.y = 0;

		distanceToTarget = _targetMovementPosition.magnitude;
		if (distanceToTarget <= movementSensitivity)
			return;
		_myRigidbody.AddForce(acceleration * Time.deltaTime * _targetMovementPosition, forceMode);
	}

	public void RotateToVelocity(float turnSpeed, bool ignoreY) {
		var velocity = _myRigidbody.velocity;
		var dir = new Vector3(velocity.x, ignoreY ? 0f : velocity.y, velocity.z);
		RotateToDirection(turnSpeed, dir,.1f);
	}

	public void RotateToDirection(float turnSpeed, Vector3 velocity, float minMag, bool ignoreY = false) {
		if (!(velocity.magnitude > minMag)) return;
		if (ignoreY)
			velocity = new Vector3(velocity.x, 0, velocity.z);
		var dirQ = Quaternion.LookRotation(velocity);
		var slerp = Quaternion.Slerp(transform.rotation, dirQ, velocity.magnitude * turnSpeed * Time.deltaTime);
		_myRigidbody.MoveRotation(slerp);
	}
	
	public void LimitSpeed(bool ignoreY)
	{
		currentSpeed = _myRigidbody.velocity;
		if (ignoreY)
			currentSpeed.y = 0;

		if (!(currentSpeed.magnitude > 0)) return;
		_myRigidbody.AddForce (-1 * deceleration * Time.deltaTime * currentSpeed, ForceMode.VelocityChange);
		if (_myRigidbody.velocity.magnitude > maxSpeed)
			_myRigidbody.AddForce (-1 * deceleration * Time.deltaTime * currentSpeed, ForceMode.VelocityChange);
	}

	public void ApplyAdditionalForce(Vector3 groundForce) => _myRigidbody.AddForce(groundForce, ForceMode.Acceleration);

	public void SetVelocity(Vector3 direction) =>
		_myRigidbody.velocity = direction;
	
	public void OverrideYVelocity(float newYVel) => _myRigidbody.OverrideYVelocity(newYVel);
	public Vector3 PlanarVelocity()
	{
		Vector3 velocity;
		return new Vector3((velocity = _myRigidbody.velocity).x, 0, velocity.z);
	}

	public float RigidbodyXZMagnitude(float min) {
		var velocity = _myRigidbody.velocity;
		var mag = new Vector3(velocity.x, 0, velocity.z).magnitude;
		return mag > min ? mag : min;
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		if (_targetMovementPosition != transform.position)
			Gizmos.DrawSphere(transform.position + _targetMovementPosition, .3f);
	}

	public void LimitVelocity()
	{
		if (_myRigidbody.velocity.magnitude > maxMagnitude)
			_myRigidbody.velocity = _myRigidbody.velocity.normalized * maxMagnitude;
	}
}
