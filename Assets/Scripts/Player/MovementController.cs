using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    public float runSpeed = 8f;
    public float walkSpeed = 4f;
    public float airSpeed = 1f;
    public float climbingSpeed = 4f;
    public float jumpDelay = 0.2f;
    public float frictionCoefficient = 15f;
    public float jumpForce = 8f;
    public float ladderJumpForce = 2f;
    public float groundingForce = 80;
    public float acceleration = 400f; 
    public float airAcceleration = 20f; 
    public float groundColliderMultiplier = .75f; 
    public float groundProbeDistance = .05f;
    public float slopeProbeDistance = 1f;
    public float maxStepHeight = 0.5f;
    public float minStepDepth = 0.2f;
    public int stepProbesCount = 3;
    public float crouchHeight = 0.9f;
    public float crouchSpeed = 0.1f;
    public Transform cameraHolder;
    public Transform model;
    public float stairsClimbingForce = 1f;
    public bool crouchSlidesEnabled = false;

    Rigidbody rb;
    CapsuleCollider cc;
    InputListener il;
    public bool grounded = false;
    public bool accelerating = false;
    public bool isSteppingUp = false;
    public bool isJumping = false;
    public bool isClimbing = false;
    public bool canBeGrounded = true;
    bool isCrouching = false;
    float jtime = 0f;
    float defaultHeight;
    float targetHeight; // The target height for the current state (crouching / standing).
    float targetYOffset; // The target y-offset for the current state.
    float targetYScale;
    Vector3 originalCameraHolderPosition;
    float originalModelScaleY;
    public float maxspd = 0f;
    public float currentAccel = 0f;
    Vector3 moveDir = Vector3.zero;
    Vector3 surfaceNormal = Vector3.up;
    int mask;

    void Start() {
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();

        defaultHeight = cc.height;
        targetHeight = defaultHeight;
        targetYOffset = 0f;

        originalCameraHolderPosition = cameraHolder.localPosition;
        originalModelScaleY = model.transform.localScale.y;

        il = GetComponent<InputListener>();
        
        string[] ignoredLayers = new string[] { "Tools", "Projectiles", "Trigger", "Smoke" };
        mask = ~LayerMask.GetMask(ignoredLayers);
    }

    void LateUpdate() {
        transform.Rotate(Vector3.up * il.GetCameraHorizontal());
    }

    void FixedUpdate() {
        ProcessCrouching();
        UpdateGroundedStatusAndMoveDirection();
        StairMovement();
        
        RegularMovement();
        
        AttemptJump();

        ExecuteMovements();
    }

    void ProcessCrouching() {
        cc.height = Mathf.Lerp(cc.height, targetHeight, crouchSpeed);
        AdjustHolderPositions(Mathf.Clamp01(Mathf.Abs((cc.height - defaultHeight) / (crouchHeight - defaultHeight))));

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
            if (!Physics.Raycast(transform.position, Vector3.up, defaultHeight - crouchHeight, mask)) {
                targetHeight = defaultHeight;
                targetYOffset = 0f;
                targetYScale = originalModelScaleY;
                isCrouching = false;
            }
        }
    }

    void UpdateGroundedStatusAndMoveDirection() {
        RaycastHit hit;

        grounded = Physics.SphereCast(
            transform.position,
            groundColliderMultiplier * cc.radius, 
            Vector3.down,
            out hit,
            cc.bounds.extents.y + groundProbeDistance,
            mask);
        
        rb.useGravity = (!grounded && !isClimbing) || (crouchSlidesEnabled && isCrouching);
            
        surfaceNormal = GetSurfaceNormalInPoint(transform.position, cc.bounds.extents.y + slopeProbeDistance);
        Debug.DrawRay(transform.position, surfaceNormal * 2f, Color.green);

        Quaternion planeRotation = Quaternion.FromToRotation(Vector3.up, surfaceNormal);

        moveDir = 
            planeRotation * 
            (rb.transform.right * il.GetInputHorizontal() + rb.transform.forward * il.GetInputVertical()).normalized;
        
        maxspd = grounded ?
            ((!il.GetIsWalking() && !il.GetIsCrouching() && IsMovingForward()) ? runSpeed : walkSpeed) 
            : isClimbing ? climbingSpeed : airSpeed;
        
        currentAccel = grounded ? acceleration : airAcceleration;
    }

    Vector3 GetSurfaceNormalInPoint(Vector3 position, float distance) {
        Debug.DrawRay(position, isClimbing ? rb.transform.forward * distance : Vector3.down, Color.cyan, 1f);
        
        RaycastHit hit;
        if (Physics.Raycast(
            position,
            isClimbing ? rb.transform.forward : Vector3.down,
            out hit,
            distance,
            mask))
        {
            canBeGrounded = true;
            return hit.normal;
        } else {
            canBeGrounded = false;
            return Vector3.up;
        }
    }

    bool IsMovingForward() {
        return Vector3.Dot(rb.transform.forward, moveDir) > 0.5f;
    }

    void StairMovement() {
        isSteppingUp = false;
        
        Vector3 stairProbeOrigin = new Vector3(
            transform.position.x, 
            transform.position.y - cc.bounds.extents.y, 
            transform.position.z);
        float probeLen = minStepDepth + cc.radius;
        RaycastHit hit;

        bool bumpingStep = false;
        for (int i = 0; i < stepProbesCount; i++)
        {
            Vector3 probeOrigin = stairProbeOrigin + (Vector3.up * maxStepHeight / (stepProbesCount + 1)) * (i + 1);

            if (Physics.Raycast(
                probeOrigin, 
                moveDir, 
                out hit, 
                probeLen, 
                mask))
            {
                bumpingStep = true;
                Debug.DrawRay(probeOrigin, moveDir * probeLen, Color.red, 1f);
                break;
            }
        }

        if (bumpingStep) {
            Vector3 upperProbeOrigin = new Vector3(
                transform.position.x, 
                transform.position.y - cc.bounds.extents.y + maxStepHeight, 
                transform.position.z);
            
            bool upperProbe = !Physics.Raycast(
                upperProbeOrigin, 
                moveDir, 
                out hit, 
                probeLen,
                mask);

            if (upperProbe) {
                Debug.DrawRay(upperProbeOrigin, moveDir * probeLen, Color.red, 1f);

                isSteppingUp = true;
            }
        }
    }

    void RegularMovement() {
        Vector3 v = Vector3.ProjectOnPlane(rb.velocity, surfaceNormal);
        float velocityInDirection = Vector3.Dot(v, moveDir);
        
        float dv = maxspd - velocityInDirection;
        accelerating = Vector3.zero != moveDir && dv > 0;
        
        if (accelerating) {
            // Calculate necessary acceleration and then force
            float requiredAccel = dv / Time.fixedDeltaTime;
            requiredAccel = requiredAccel > currentAccel ? currentAccel : requiredAccel;
            
            Accelerate(moveDir, requiredAccel);

            Debug.DrawRay(transform.position, moveDir * requiredAccel, Color.green, 1f);
        }
    }

    void AttemptJump() {
        isJumping = false;

        if ((grounded || isClimbing) && isJumpReady()) {
            if (il.GetIsJumping()) {
                jtime = Time.time;
                isJumping = true;
            }
        }
    }

    bool isJumpReady() {
        return (Time.time - jtime) > jumpDelay;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Ladder")) {
            isClimbing = true;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Ladder")) {
            isClimbing = false;
        }
    }

    void ExecuteMovements() {
        if (isJumping) {
            Throw(isClimbing ? -rb.transform.forward : Vector3.up, isClimbing ? ladderJumpForce : jumpForce);
        }

        if (isSteppingUp && !isClimbing) {
            Accelerate(Vector3.up, stairsClimbingForce);
        }

        if (canBeGrounded && !isSteppingUp && isJumpReady() && !isClimbing) {
            Accelerate(-surfaceNormal, groundingForce);
        }

        if ((grounded || isClimbing) && isJumpReady()) {
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
        Vector3 inPlainVelocity = Vector3.ProjectOnPlane(rb.velocity, surfaceNormal);
        Vector3 friction = -inPlainVelocity * frictionCoefficient;
        rb.AddForce(friction, ForceMode.Force);
        Debug.DrawRay(transform.position, -inPlainVelocity * frictionCoefficient, Color.red, 1f);
    }

    private void ApplyFrictionCounterDrift()
    {
        Vector3 inPlainVelocity = Vector3.ProjectOnPlane(rb.velocity, surfaceNormal);
        Vector3 driftDir = -(inPlainVelocity - moveDir * maxspd);

        // Apply friction proportional to the direction difference
        Vector3 friction = driftDir * frictionCoefficient;
        rb.AddForce(friction, ForceMode.Force);
        
        Debug.DrawRay(transform.position, driftDir * frictionCoefficient, Color.blue, 1f);
    }

    public void Accelerate(Vector3 direction, float acceleration)
    {
        // assuming that the direction is a unit vector
        Vector3 force = direction * acceleration;
        rb.AddForce(force, ForceMode.Force);
    }

    public void Throw(Vector3 direction, float impulseForce)
    {
        // assuming that the direction is a unit vector
        rb.AddForce(direction * impulseForce, ForceMode.Impulse);
    }
}
