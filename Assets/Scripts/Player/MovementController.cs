using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {
    public float maxrspd = 10f;
    public float maxwspd = 4f;
    public float aircdelay = 0.5f;
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


    Rigidbody rb;
    CapsuleCollider cc;
    InputListener il;

    bool grounded = false;
    bool isCrouching = false;
    float totime = 0f;
    float jtime = 0f;

    float defaultDrag = 0f;
    float maxspd = 0;
    float defaultHeight;
    float targetHeight; // The target height for the current state (crouching / standing).
    float targetYOffset; // The target y-offset for the current state.

    // Add fields to store original positions.
    Vector3 originalCameraHolderPosition;
    Vector3 originalToolHolderPosition;
    Vector3 originalModelPosition;


    void Start() {
        rb = GetComponent<Rigidbody>();
        defaultDrag = rb.drag;
        cc = GetComponent<CapsuleCollider>();

        defaultHeight = cc.height;
        targetHeight = defaultHeight;
        targetYOffset = 0f;

        originalCameraHolderPosition = cameraHolder.localPosition;
        originalToolHolderPosition = toolHolder.localPosition;
        originalModelPosition = model.localPosition;

        il = GetComponent<InputListener>();
    }

    void LateUpdate() {
        transform.Rotate(Vector3.up * il.GetCameraHorizontal());
    }

    void FixedUpdate() 
    {
        ProcessCrouching();
        Vector3 moveDir = CalculateMoveDirection();
        UpdateGroundedStatus();
        ApplyDragBasedOnGroundStatus();
        ApplyHorizontalMovement(moveDir);
        AttemptJump();
    }

    Vector3 CalculateMoveDirection() 
    {
        Vector3 moveDir = 
            (rb.transform.right * il.GetInputHorizontal() 
            + rb.transform.forward * il.GetInputVertical())
            .normalized * mfrc;
        maxspd = il.GetIsWalking() || il.GetIsCrouching() ? maxwspd : maxrspd; // walk/run

        return moveDir;
    }

    void UpdateGroundedStatus() 
    {
        bool wasGrounded = grounded;
        float radius = cc.radius * groundColliderMultiplier;
        float distance = cc.bounds.extents.y - radius + groundProbeDistance;
        RaycastHit hit;
        grounded = Physics.SphereCast(rb.position, radius, Vector3.down, out hit, distance);

        if (!grounded && wasGrounded) 
        {
            totime = Time.time;
        }
    }

    void ApplyDragBasedOnGroundStatus() 
    {
        if (grounded) 
        {
            rb.drag = bfactor;
        } 
        else 
        {
            rb.drag = defaultDrag;
        }
    }

    void ApplyHorizontalMovement(Vector3 moveDir) 
    {
        bool hasAirCountrol = (Time.time - totime) <= aircdelay;
        if (grounded || hasAirCountrol) 
        {
            if (moveDir != Vector3.zero && rb.velocity.magnitude < maxspd) 
            {
                rb.drag = defaultDrag;
                rb.AddForce(moveDir, ForceMode.Force);
            }
        }
    }

    void AttemptJump() 
    {
        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) 
        {
            if (il.GetIsJumping()) 
            {
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

    void AdjustHolderPositions(float t)
    {
        // Interpolate positions between original and offset values.
        cameraHolder.localPosition = Vector3.Lerp(originalCameraHolderPosition, new Vector3(originalCameraHolderPosition.x, originalCameraHolderPosition.y + targetYOffset, originalCameraHolderPosition.z), t);
        toolHolder.localPosition = Vector3.Lerp(originalToolHolderPosition, new Vector3(originalToolHolderPosition.x, originalToolHolderPosition.y + targetYOffset, originalToolHolderPosition.z), t);
        model.localPosition = Vector3.Lerp(originalModelPosition, new Vector3(originalModelPosition.x, originalModelPosition.y + targetYOffset, originalModelPosition.z), t);
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
