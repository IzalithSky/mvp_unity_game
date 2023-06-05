using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    public float maxrspd = 8f;
    public float maxwspd = 4f;
    public float maxaspd = 0.1f;
    public float jdelay = 0.2f;
    public float bfactor = 15f;
    public float jfrc = 5f;
    public float mfrc = 50f; 
    public float groundColliderMultiplier = .75f; 
    public float groundProbeDistance = .05f;
    public float crouchHeight = 0.9f;
    public float crouchSpeed = 0.1f;
    public Transform cameraHolder;
    public Transform toolHolder;
    public Transform model;
    public float maxStepHeight = 0.3f;

    Rigidbody rb;
    CapsuleCollider cc;
    InputListener il;
    bool grounded = false;
    bool wasGrounded = false;
    bool isCrouching = false;
    float totime = 0f;
    float jtime = 0f;
    float defaultDrag = 0f;
    float defaultHeight;
    float targetHeight; // The target height for the current state (crouching / standing).
    float targetYOffset; // The target y-offset for the current state.
    Vector3 originalCameraHolderPosition;
    Vector3 originalModelPosition;
    bool applyingSlopeForce = false;
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
        ApplyDragBasedOnGroundStatus();
        ApplyHorizontalMovement();
        AttemptJump();
    }

    void CalculateMoveDirectionAndMaxSpeed() {
        moveDir = 
            (rb.transform.right * il.GetInputHorizontal() 
            + rb.transform.forward * il.GetInputVertical())
            .normalized;
    }

    void UpdateGroundedStatus() {
        wasGrounded = grounded;
        float radius = cc.radius * groundColliderMultiplier;
        float distance = cc.bounds.extents.y - radius + groundProbeDistance;
        RaycastHit hit;
        grounded = Physics.SphereCast(rb.position, radius, Vector3.down, out hit, distance);

        if (!grounded && wasGrounded)  {
            totime = Time.time;
        }

        maxspd = (grounded && wasGrounded) ? (il.GetIsWalking() || il.GetIsCrouching() ? maxwspd : maxrspd) : maxaspd;
    }

    void ApplyDragBasedOnGroundStatus() {
        if (grounded) {
            rb.drag = bfactor;
        } else {
            rb.drag = defaultDrag;
        }
    }

    void ApplyHorizontalMovement() {
        SlopeMovement();
        RegularMovement();
    }

    void RegularMovement() {
        Vector3 v = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float velocityInDirection = Vector3.Dot(v, moveDir);
        
        float addVel = maxspd - velocityInDirection;
        
        if (addVel <= 0) {
            return;
        }
        
        rb.AddForce(moveDir * mfrc, ForceMode.Acceleration);
    }

    void SlopeMovement() {
        applyingSlopeForce = false;
        Vector3 feetPosition = rb.position - new Vector3(0f, (cc.height / 2f), 0f);
        RaycastHit hit;
        Vector3 direction = moveDir;
        float moveDist = maxspd * Time.deltaTime;
        if (Physics.SphereCast(
            feetPosition + new Vector3(0f, cc.radius, 0f), 
            cc.radius, 
            direction, 
            out hit, 
            moveDist, 
            ~0, 
            QueryTriggerInteraction.Ignore))
        {
            if (hit.point.y - feetPosition.y <= maxStepHeight) {
                rb.AddForce(Vector3.up * Physics.gravity.magnitude * 10.1f, ForceMode.Acceleration);
                applyingSlopeForce = true;
            }
        } else { 
            if (!applyingSlopeForce && 
                Physics.Raycast(
                feetPosition, 
                -direction, 
                moveDist, 
                ~0, 
                QueryTriggerInteraction.Ignore))
            {
                rb.AddForce(Vector3.down * Physics.gravity.magnitude, ForceMode.Acceleration);
                applyingSlopeForce = true;
            }
        }
    }

    void AttemptJump() {
        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) {
            if (il.GetIsJumping()) {
                jtime = Time.time;
                rb.drag = defaultDrag;
                rb.AddForce(rb.transform.up * jfrc, ForceMode.Impulse);
            }
        }
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
}
