using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerCc : MonoBehaviour {
    public float maxrspd = 8f;
    public float maxwspd = 4f;
    public float maxaspd = 1f;
    public float maxcspd = 4f;
    public float jdelay = 0.2f;
    public float frictionCoefficient = 15f;
    public float jfrc = 5f;
    public float accel = 400f; 
    public float airaccel = 20f; 
    public float crouchHeight = 0.9f;
    public float crouchSpeed = 0.1f;
    public Transform cameraHolder;
    public Transform model;
    public float stairsClimbingAcceleration = 1f;
    public bool crouchSlidesEnabled = false;

    public CharacterController characterController;
    InputListener il;
    public bool grounded = false;
    public bool accelerating = false;
    bool isCrouching = false;
    float jtime = 0f;
    float defaultHeight;
    float targetHeight; // The target height for the current state (crouching / standing).
    float targetYOffset; // The target y-offset for the current state.
    float targetYScale;
    Vector3 originalCameraHolderPosition;
    float originalModelScaleY;
    float maxspd = 0f;
    float currentAccel = 0f;
    Vector3 moveDir = Vector3.zero;
    bool wasGrounded = false;
    bool jumpStarted = false;
    public Vector3 characterVelocity = Vector3.zero;
    public float velocityInDirection = 0f;


    void Start() {
        characterController = GetComponent<CharacterController>();

        defaultHeight = characterController.height;
        targetHeight = defaultHeight;
        targetYOffset = 0f;

        originalCameraHolderPosition = cameraHolder.localPosition;
        originalModelScaleY = model.transform.localScale.y;

        il = GetComponent<InputListener>();
    }

    void LateUpdate() {
        transform.Rotate(Vector3.up * il.GetCameraHorizontal());
    }

    void Update() {
        ProcessCrouching();
        UpdateGroundedStatusAndMoveDirection();
        ApplyGravity();
        RegularMovement();
        AttemptJump();
        DoFriction();
        MoveCharacter();
    }

    void ProcessCrouching() {
        characterController.height = Mathf.Lerp(characterController.height, targetHeight, crouchSpeed);
        AdjustHolderPositions(Mathf.Clamp01(Mathf.Abs((characterController.height - defaultHeight) / (crouchHeight - defaultHeight))));

        if (il.GetIsCrouching()) {
            Crouch();
        } else {
            StandUp();
        }
    }

    void AdjustHolderPositions(float t) {
        cameraHolder.localPosition = Vector3.Lerp(
            originalCameraHolderPosition, 
            new Vector3(originalCameraHolderPosition.x, originalCameraHolderPosition.y + targetYOffset, originalCameraHolderPosition.z), 
            t);

        model.localScale = new Vector3(
            model.localScale.x, 
            Mathf.Lerp(model.localScale.y, targetYScale, t), 
            model.localScale.z);
    }

    void Crouch() {
        if (!isCrouching) {
            targetHeight = crouchHeight;
            targetYOffset = -(defaultHeight - crouchHeight) / 2;
            targetYScale = crouchHeight / defaultHeight;
            isCrouching = true;
        }
    }

    void StandUp() {
        if (isCrouching) {
            if (!Physics.Raycast(transform.position, Vector3.up, defaultHeight - crouchHeight)) {
                targetHeight = defaultHeight;
                targetYOffset = 0f;
                targetYScale = originalModelScaleY;
                isCrouching = false;
            }
        }
    }

    void UpdateGroundedStatusAndMoveDirection() {
        wasGrounded = grounded;
        grounded = characterController.isGrounded;

        if (jumpStarted && grounded && !wasGrounded) {
            jumpStarted = false;
        }

        // CalculateMoveDirectionAndMaxSpeed
        moveDir = (characterController.transform.right * il.GetInputHorizontal() + characterController.transform.forward * il.GetInputVertical()).normalized;
        maxspd = grounded && (!il.GetIsWalking() && !il.GetIsCrouching() && IsMovingForward()) ? maxrspd : maxwspd;
        currentAccel = grounded ? accel : airaccel;
        
        if (jumpStarted) {
            currentAccel = airaccel;
            maxspd = maxaspd;
        }
    }

    bool IsMovingForward() {
        return Vector3.Dot(characterController.transform.forward, moveDir) > 0.5f;
    }

    void RegularMovement() {
        Vector3 v = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        velocityInDirection = Vector3.Dot(v, moveDir);
        
        float dv = maxspd - velocityInDirection;
        accelerating = Vector3.zero != moveDir && dv > 0;
        
        if (accelerating) {
            // Calculate necessary acceleration and then force
            float requiredAccel = dv / Time.deltaTime;
            requiredAccel = requiredAccel > currentAccel ? currentAccel : requiredAccel;
            Accelerate(moveDir, requiredAccel);
        }
    }

    void AttemptJump() {
        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) {
            if (il.GetIsJumping()) {
                jumpStarted = true;
                jtime = Time.time;

                Throw(Vector3.up, jfrc);
            }
        }
    }

    void DoFriction() {
        if (grounded && !jumpStarted) {
            if (!(crouchSlidesEnabled && isCrouching)) {
                if (Vector3.zero == moveDir) {
                    ApplyFriction();
                } else {
                    ApplyFrictionCounterDrift();
                }
            }
        }
    }

    private void ApplyFriction()
    {
        Vector3 horizontalVelocity = new Vector3(characterVelocity.x, 0, characterVelocity.z);
        Vector3 friction = -horizontalVelocity * frictionCoefficient;
        characterVelocity += friction * Time.deltaTime;
    }

    private void ApplyFrictionCounterDrift()
    {
        Vector3 currentDirection = new Vector3(characterVelocity.x, 0, characterVelocity.z);
        Vector3 driftDir = -(currentDirection - moveDir * maxspd);

        // Apply friction proportional to the direction difference
        Vector3 friction = driftDir * frictionCoefficient;
        characterVelocity += friction * Time.deltaTime;
    }

    public void Accelerate(Vector3 direction, float acceleration)
    {
        // assuming that the direction is a unit vector
        Vector3 force = direction * acceleration;
        characterVelocity += force * Time.deltaTime;
    }

    public void ApplyGravity() {
        if (grounded) {
            characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);
        } else {
            characterVelocity += Physics.gravity * Time.deltaTime;
        }
    }

    public void Throw(Vector3 direction, float impulseForce)
    {
        // assuming that the direction is a unit vector
        characterVelocity += direction * impulseForce;
    }

    void MoveCharacter() {
        characterController.Move(characterVelocity);
    }
}
