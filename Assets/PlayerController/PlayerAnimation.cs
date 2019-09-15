using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")] 
    CharacterMotor _characterMotor;
    GroundDetection _groundDetection;

    [Header("Ground Alignment")]
    [SerializeField] bool alignToGround = true;
    [SerializeField] float groundAlignmentSpeed = 4f;
    [SerializeField] Transform model;
    [SerializeField] Vector3 upVector = Vector3.up;
    [SerializeField] float slopeLookForwardSpeed = 4f;
    [SerializeField] bool lookForward; // TODO: misleading
    public bool OnBack;

    [Header("Ground Particles")]
    [SerializeField] ParticleSystem groundParticleSystem;
    [SerializeField] ParticleSystem.EmissionModule groundParticleEmission;
    [SerializeField] float targetEmissionRate;
    [SerializeField] float maxEmissionRate = 10f;
    [SerializeField] float emissionRateChangeSpeed = 50f;
    [SerializeField] float minSpeedForGroundParticleEmission;
    [SerializeField] float velocityChangeThreshold = .1f;
    [SerializeField] int velocityChangeParticleCount = 100;

    //TODO: debug variables?
    Vector3 planerVelocityLastFrame;
    public float velocityDif;

    void Awake()
    {
        _characterMotor = GetComponent<CharacterMotor>();
        _groundDetection = GetComponent<GroundDetection>();
        model = transform.GetChild(0);
        groundParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    void OnEnable() => groundParticleEmission = groundParticleSystem.emission;

    void Update()
    {
        AttemptToAlignToGround();
        AttemptToAnimateGroundParticles();
    }

    void AttemptToAlignToGround()
    {
        if (alignToGround) AlignToGround();
    }
    
    public void AlignToGround()
    {
        upVector = OnBack ? transform.forward : transform.up;
        var newRotation = 
            Quaternion.FromToRotation(upVector, _groundDetection.GroundRelationship_.groundNormal) * transform.rotation; 
        model.rotation = Quaternion.Lerp(model.rotation, newRotation, groundAlignmentSpeed * Time.deltaTime);
        if (lookForward || OnBack)
            _characterMotor.RotateToVelocity(slopeLookForwardSpeed, false);
    }
    
    void AttemptToAnimateGroundParticles()
    {
        if (groundParticleEmission.enabled)
            AnimateGroundParticlesEmission();
    }

    void AnimateGroundParticlesEmission()
    {
        DetermineGroundParticleEmissionRate();
        LerpGroundParticleEmissionRate();
    }

    void LerpGroundParticleEmissionRate()
    {
        groundParticleEmission.rateOverDistanceMultiplier = Mathf.Lerp(
            groundParticleEmission.rateOverDistanceMultiplier, 
            targetEmissionRate,
            emissionRateChangeSpeed * Time.deltaTime);
    }

    void DetermineGroundParticleEmissionRate()
    {
        var speed = _characterMotor.RigidbodyXZMagnitude(0);
        velocityDif = (_characterMotor.PlanarVelocity() - planerVelocityLastFrame).magnitude;
        if (_groundDetection.GroundRelationship_.grounded)
        {
             if (speed > minSpeedForGroundParticleEmission)
                 targetEmissionRate = maxEmissionRate;
             else if (velocityDif > velocityChangeThreshold)
                 groundParticleSystem.Emit(
                     new ParticleSystem.EmitParams
                     {
                         velocity = planerVelocityLastFrame + transform.right * Random.Range(-1f,1f),
                         startSize = Random.Range(.1f,.5f)
                     },
                     velocityChangeParticleCount);
             else
                 targetEmissionRate = 0; 
        } else
             targetEmissionRate = 0;
    }
    
    void LateUpdate() => planerVelocityLastFrame = _characterMotor.PlanarVelocity();
}
