using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementControllerAlt : MonoBehaviour {
    public float maxrspd = 8f;
    public float maxwspd = 4f;
    public float maxaspd = 1f;
    public float maxcspd = 4f;
    public float jdelay = 0.2f;
    public float frictionCoefficient = 15f;
    public float jfrc = 5f;
    public float accel = 400f; 
    public float airaccel = 20f; 
    public float groundColliderMultiplier = .75f; 
    public float groundProbeDistance = .05f;
    public float slopeProbeDistance = 1f;
    public float crouchHeight = 0.9f;
    public float crouchSpeed = 0.1f;
    public Transform cameraHolder;
    public Transform model;
    public float stairsClimbingAcceleration = 1f;
    public bool crouchSlidesEnabled = false;

    Rigidbody rb;
    CapsuleCollider cc;
    InputListener il;
    public bool grounded = false;
    public bool wasGrounded = false;
    public bool jumpStarted = false;
    public bool accelerating = false;
    public bool bumpingStep = false;
    public bool isClimbing = false;
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
    public float slopeAngle = 0f;
    Vector3 surfaceNormal = Vector3.up;
    int mask;
    int cnt = 0;

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
        // StairMovement();
        RegularMovement();
        AttemptJump();
        DoFriction();
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

        RaycastHit hit;

        grounded = Physics.SphereCast(
            transform.position,
            groundColliderMultiplier * cc.radius, 
            Vector3.down,
            out hit,
            cc.bounds.extents.y + groundProbeDistance,
            mask);
            
        surfaceNormal = GetSurfaceNormalInPoint(transform.position, cc.bounds.extents.y + slopeProbeDistance);
        slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);

        rb.useGravity = !grounded;

        // CalculateMoveDirectionAndMaxSpeed
        moveDir = (rb.transform.right * il.GetInputHorizontal() + rb.transform.forward * il.GetInputVertical()).normalized;
        maxspd = grounded && (!il.GetIsWalking() && !il.GetIsCrouching() && IsMovingForward()) ? maxrspd : maxwspd;
        currentAccel = grounded ? accel : airaccel;
    }

    Vector3 GetSurfaceNormalInPoint(Vector3 position, float distance) {
        RaycastHit hit;
        if (Physics.Raycast(
            position,
            Vector3.down,
            out hit,
            distance,
            mask))
        {
            return hit.normal;
        } else {
            return Vector3.up;
        }
    }

    bool IsMovingForward() {
        return Vector3.Dot(rb.transform.forward, moveDir) > 0.5f;
    }

    Vector3 ProjectVectorOntoPlane(Vector3 vector, Vector3 planeNormal)
    {
        // Calculate the dot product of the vector and the plane normal
        float dotProduct = Vector3.Dot(vector, planeNormal);

        // Calculate the projection of the vector onto the plane
        Vector3 projection = vector - dotProduct * planeNormal;

        return projection;
    }

    void StairMovement() {
        Vector3 stairProbeOrigin = new Vector3(
            transform.position.x, 
            transform.position.y - cc.bounds.extents.y + cc.radius, 
            transform.position.z);
        float probeLen = (rb.velocity.magnitude * Time.fixedDeltaTime) + cc.radius;
        RaycastHit hit;

        bumpingStep = Physics.SphereCast(
            stairProbeOrigin, 
            groundColliderMultiplier * cc.radius, 
            moveDir, 
            out hit, 
            probeLen,
            mask);

        Debug.DrawRay(stairProbeOrigin, moveDir * probeLen, Color.red, 1f);

        float vv = rb.velocity.y;

        if (bumpingStep) {
            if (grounded || isClimbing) {
                if (vv < maxcspd) {
                    Accelerate(Vector3.up, stairsClimbingAcceleration);
                }
            }
        }
    }

    void RegularMovement() {
        Vector3 v = ProjectVectorOntoPlane(rb.velocity, surfaceNormal);
        float velocityInDirection = Vector3.Dot(v, moveDir);
        
        float dv = maxspd - velocityInDirection;
        accelerating = Vector3.zero != moveDir && dv > 0;
        
        if (accelerating) {
            // Calculate necessary acceleration and then force
            float requiredAccel = dv / Time.fixedDeltaTime;
            requiredAccel = requiredAccel > currentAccel ? currentAccel : requiredAccel;
            
            Vector3 nv = ProjectVectorOntoPlane(moveDir, surfaceNormal).normalized;
            Accelerate(nv, requiredAccel);

            Debug.DrawRay(transform.position, nv * requiredAccel, Color.green, 1f);
        }
    }

    void AttemptJump() {
        cnt += 1;
        cnt = cnt % 1000;

        if (jumpStarted && grounded && !wasGrounded) {
            jumpStarted = false;
        }

        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) {
            if (il.GetIsJumping()) {
                jumpStarted = true;
                jtime = Time.time;

                Debug.Log("jump, grounded " + grounded + ", wasgrounded " + wasGrounded + ", jumpstarted " + jumpStarted + " " + cnt);
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

    private void ApplyFriction()
    {
        Vector3 inPlainVelocity = ProjectVectorOntoPlane(rb.velocity, surfaceNormal);
        Vector3 friction = -inPlainVelocity * frictionCoefficient;
        rb.AddForce(friction, ForceMode.Force);
        Debug.DrawRay(transform.position, -inPlainVelocity * frictionCoefficient, Color.red, 1f);
    }

    private void ApplyFrictionCounterDrift()
    {
        Vector3 inPlainVelocity = ProjectVectorOntoPlane(rb.velocity, surfaceNormal);
        Vector3 driftDir = -(inPlainVelocity - ProjectVectorOntoPlane(moveDir, surfaceNormal).normalized * maxspd);

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
