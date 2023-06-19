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
    public float maxStepHeight = 0.3f;
    public float stairsClimbingAcceleration = 1f;
    public bool crouchSlidesEnabled = false;
    public float crouchJumpMult = 0.7f;

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
    Vector3 moveDir = Vector3.zero;


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
    }

    void LateUpdate() {
        transform.Rotate(Vector3.up * il.GetCameraHorizontal());
    }

    void FixedUpdate() {
        ProcessCrouching();
        CalculateMoveDirectionAndMaxSpeed();
        UpdateGroundedStatus();
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

    void CalculateMoveDirectionAndMaxSpeed() {
        moveDir = 
            (rb.transform.right * il.GetInputHorizontal() 
            + rb.transform.forward * il.GetInputVertical())
            .normalized;
    }

    void UpdateGroundedStatus() {
        RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("Trigger"));
        grounded = Physics.SphereCast(
            transform.position, 
            groundColliderMultiplier * cc.radius, 
            Vector3.down, 
            out hit, 
            cc.bounds.extents.y + groundProbeDistance,
            layerMask);

        if (grounded) {
            if (crouchSlidesEnabled) {
                rb.drag = isCrouching ? defaultDrag : bfactor;
            } else {
                rb.drag = bfactor;
            }
        } else {
            rb.drag = defaultDrag;
        }

        maxspd = grounded ? 
            (il.GetIsWalking() || il.GetIsCrouching() ? maxwspd : (IsMovingForward() ? maxrspd : maxwspd)) : 
            maxaspd;

        if (isClimbing) {
            rb.useGravity = false;
            grounded = false;
            return;
        } else {
            rb.useGravity = true;
        }
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
        
        accelerating = Vector3.zero != moveDir && velocityInDirection < maxspd;
        if (accelerating) {
            rb.AddForce(moveDir * (grounded ? accel : airaccel), ForceMode.Acceleration);
        }
    }

    void AttemptJump() {
        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) {
            if (il.GetIsJumping()) {
                jtime = Time.time;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(rb.transform.up * jfrc * (isCrouching && crouchSlidesEnabled ? crouchJumpMult : 1f), ForceMode.Impulse);
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
