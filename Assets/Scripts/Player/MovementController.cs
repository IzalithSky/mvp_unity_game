using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    public float maxrspd = 8f;
    public float maxwspd = 4f;
    public float maxaspd = 1f;
    public float maxcspd = 4f;
    public float jdelay = 0.2f;
    public float bfactor = 15f;
    public float jfrc = 5f;
    public float accel = 400f; 
    public float airaccel = 20f; 
    public float groundColliderMultiplier = .75f; 
    public float groundProbeDistance = .05f;
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
    public bool accelerating = false;
    public bool bumpingStep = false;
    public bool bumpingStepBack = false;
    public bool isClimbing = false;
    bool isCrouching = false;
    float jtime = 0f;
    float defaultDrag = 0f;
    float defaultHeight;
    float targetHeight; // The target height for the current state (crouching / standing).
    float targetYOffset; // The target y-offset for the current state.
    Vector3 originalCameraHolderPosition;
    Vector3 originalModelPosition;
    float maxspd = 0f;
    float currentAccel = 0f;
    Vector3 moveDir = Vector3.zero;
    bool wasGrounded = false;
    bool jumpStarted = false;

    string[] ignoredLayers = new string[] { "Tools", "Projectiles", "Trigger", "Smoke" };
    int mask;

    void Start() {
        rb = GetComponent<Rigidbody>();
        defaultDrag = rb.drag;
        cc = GetComponent<CapsuleCollider>();

        defaultHeight = cc.height;
        targetHeight = defaultHeight;
        targetYOffset = 0f;

        originalCameraHolderPosition = cameraHolder.localPosition;
        originalModelPosition = model.localPosition;

        il = GetComponent<InputListener>();

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

        model.localPosition = Vector3.Lerp(
            originalModelPosition, 
            new Vector3(originalModelPosition.x, originalModelPosition.y + targetYOffset, originalModelPosition.z), 
            t);
    }

    void Crouch() {
        if (!isCrouching) {
            targetHeight = crouchHeight;
            targetYOffset = -(defaultHeight - crouchHeight) / 2;
            isCrouching = true;
        }
    }

    void StandUp() {
        if (isCrouching) {
            if (!Physics.Raycast(transform.position, Vector3.up, defaultHeight - crouchHeight)) {
                targetHeight = defaultHeight;
                targetYOffset = 0f;
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

        if (jumpStarted && grounded && !wasGrounded) {
            jumpStarted = false;
        }

        // CalculateMoveDirectionAndMaxSpeed
        moveDir = (rb.transform.right * il.GetInputHorizontal() + rb.transform.forward * il.GetInputVertical()).normalized;
        maxspd = grounded && (!il.GetIsWalking() && !il.GetIsCrouching() && IsMovingForward()) ? maxrspd : maxwspd;
        currentAccel = grounded ? accel : airaccel;
        rb.drag = (grounded && (!crouchSlidesEnabled || isCrouching)) ? bfactor : defaultDrag;
        rb.useGravity = !isClimbing;

        grounded = isClimbing ? false : grounded;
    }

    bool IsMovingForward() {
        return Vector3.Dot(rb.transform.forward, moveDir) > 0f;
    }

    void StairMovement() {
        Vector3 stairProbeOrigin = new Vector3(
            transform.position.x, 
            transform.position.y - cc.bounds.extents.y + cc.radius, 
            transform.position.z);
        float probeLen = (maxrspd * Time.fixedDeltaTime) + cc.radius;
        RaycastHit hit;

        bumpingStep = Physics.SphereCast(
            stairProbeOrigin, 
            groundColliderMultiplier * cc.radius, 
            moveDir, 
            out hit, 
            probeLen);
        float vv = rb.velocity.y;

        if (bumpingStep) {
            if (grounded || isClimbing) {
                if (vv < maxcspd) {
                    rb.AddForce(Vector3.up * stairsClimbingAcceleration, ForceMode.Acceleration);
                }
            }
        } else {
            bumpingStepBack = Physics.SphereCast(
                stairProbeOrigin, 
                groundColliderMultiplier * cc.radius, 
                -moveDir, 
                out hit, 
                probeLen);

            if (bumpingStepBack) {
                rb.AddForce(Vector3.down * stairsClimbingAcceleration, ForceMode.Acceleration);
            }
        }
    }

    void RegularMovement() {
        Vector3 v = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float velocityInDirection = Vector3.Dot(v, moveDir);
        
        float dv = maxspd - velocityInDirection;
        accelerating = Vector3.zero != moveDir && dv > 0;
        if (accelerating) {
            // Calculate necessary acceleration and then force
            float requiredAccel = dv / Time.fixedDeltaTime;
            requiredAccel = requiredAccel > currentAccel ? currentAccel : requiredAccel;
            Vector3 requiredForce = moveDir * requiredAccel * rb.mass;
            rb.AddForce(requiredForce, ForceMode.Force);
        }
    }

    void AttemptJump() {
        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) {
            if (il.GetIsJumping()) {
                jumpStarted = true;
                jtime = Time.time;

                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                rb.AddForce(rb.transform.up * jfrc, ForceMode.Impulse);
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
}
