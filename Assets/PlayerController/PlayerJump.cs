using System;
using Extensions;
using UnityEngine;

namespace PlayerController
{
    public class PlayerJump : InputListener
    {
        CharacterMotor _characterMotor;
        GroundDetection _groundDetection;
        PlayerMove _playerMove;

        [Header("Config")]
        [SerializeField] float extraGravitySmoothTime = 2.5f;
        
        [Header("Jump Leniency")]
        [SerializeField] bool recentlyGrounded;
        [SerializeField] bool recentGroundAngle;
        [SerializeField] float jumpLeniency = 0.17f;

        [Header("Debug")]
        [ReadOnly] [SerializeField] float currentExtraGravity;
        [ReadOnly] [SerializeField] MovementProfile currentProfile;
        public MovementProfile CurrentProfile { set => currentProfile = value; }

        protected override void SubscribeToInputListeners()
        {
            PlayerInput.OnJumpPress += JumpPressed;
            PlayerInput.OnJumpRelease += JumpReleased;
        }

        protected override void UnsubscribeFromInputListeners()
        {
            PlayerInput.OnJumpPress -= JumpPressed;
            PlayerInput.OnJumpRelease -= JumpReleased;
        }

        void Awake()
        {
            _characterMotor = GetComponent<CharacterMotor>();
            _groundDetection = GetComponent<GroundDetection>();
            _playerMove = GetComponent<PlayerMove>();
        }

        void JumpPressed()
        {
            if (!EligibleForJump()) return;
            StopJumpLeniency();
            Jump();
            currentExtraGravity = 0;
        }
        
        bool EligibleForJump() => _groundDetection.CurrentlyOnValidGround(_playerMove.MaxWalkableSlopeAngle) || RecentlyOnValidGround();


        void Jump() => Jump(new Vector3(0, currentProfile.jumpConfig ? currentProfile.jumpConfig.jumpForce : 0, 0));
	
        void Jump(Vector3 direction) {
            _characterMotor.OverrideYVelocity(0);
            _characterMotor.AddRelativeForce(direction, ForceMode.Impulse);
        }

        bool RecentlyOnValidGround() => recentlyGrounded && recentGroundAngle;
        
        void JumpReleased()
        {
            if (!currentProfile.ascendingConfig) return;
            if (_characterMotor.CurrentVelocity.y > currentProfile.ascendingConfig.jumpReleaseVelocity)
                _characterMotor.OverrideYVelocity(currentProfile.ascendingConfig.jumpReleaseVelocity);
        }
        
        Vector3 ExtraGravity()
        {
            currentExtraGravity =
                Mathf.Lerp(
                    currentExtraGravity, 
                    currentProfile.additionalGravityConfig ? currentProfile.additionalGravityConfig.gravity : 0, 
                    extraGravitySmoothTime * Time.deltaTime
                    );
            return currentExtraGravity * Time.deltaTime * Vector3.down;
        }

        void FixedUpdate()
        {
            CalculateGroundedState();

            if (IsFalling())
                _characterMotor.MoveVertical(ExtraGravity());
        }

        void CalculateGroundedState()
        {
            var grounded = _groundDetection.GroundRelationship_.grounded;
            var groundedStateLastFrame = grounded;
            if (!_groundDetection.GroundRelationship_.grounded && groundedStateLastFrame)
                EnactJumpLeniency();
        }

        async void EnactJumpLeniency()
        {
            recentlyGrounded = true;
            recentGroundAngle = _groundDetection.GroundRelationship_.groundAngle <= _playerMove.MaxWalkableSlopeAngle;
            await TimeSpan.FromSeconds(jumpLeniency);
            StopJumpLeniency();
        }
        
        void StopJumpLeniency() => recentlyGrounded = false;

        bool IsFalling() => !_groundDetection.GroundRelationship_.grounded && _characterMotor.CurrentVelocity.y < 0;

    }

    public abstract class InputListener : MonoBehaviour
    {
        protected void OnEnable() => SubscribeToInputListeners();
        protected void OnDisable() => UnsubscribeFromInputListeners();

        protected virtual void SubscribeToInputListeners() {}
        protected virtual void UnsubscribeFromInputListeners() {}
    }
}