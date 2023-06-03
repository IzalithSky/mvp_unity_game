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
    public float slopeForce = 20f;
    public float groundColliderMultiplier = .75f; 
    public float groundProbeDistance = .05f;
    public float crouchHeight = 0.9f;
    public float crouchSpeed = 0.1f;
    public Transform cameraHolder;
    public Transform toolHolder;
    public Transform model;
    public float maxStepHeight = 0.3f;
    public float airsteerSpeed = 5f;


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

        bool applyingSlopeForce = false;
        Vector3 feetPosition = rb.position - new Vector3(0f, (cc.height / 2f), 0f);
        RaycastHit hit;
        if (Physics.SphereCast(
            feetPosition + new Vector3(0f, cc.radius, 0f), 
            cc.radius, 
            moveDir.normalized, 
            out hit, 
            moveDir.magnitude * Time.deltaTime, 
            ~0, 
            QueryTriggerInteraction.Ignore))
        {
            if (hit.point.y - feetPosition.y <= maxStepHeight )
            {
                rb.AddForce(Vector3.up * slopeForce, ForceMode.Force);
                applyingSlopeForce = true;
            }
        } else { 
            if (!applyingSlopeForce && 
                Physics.Raycast(
                feetPosition, 
                -moveDir.normalized, 
                cc.height, 
                ~0, 
                QueryTriggerInteraction.Ignore))
            {
                rb.AddForce(Vector3.down * slopeForce, ForceMode.Force);
                applyingSlopeForce = true;
            }
        }

        if (!grounded && !applyingSlopeForce) {
            Vector3 horizontalVelocity = rb.velocity;
            horizontalVelocity.y = 0;

            Vector3 newHorizontalVelocity = horizontalVelocity.magnitude * transform.forward;
            rb.velocity = new Vector3(newHorizontalVelocity.x, rb.velocity.y, newHorizontalVelocity.z);
        }
    }

    private void OnDrawGizmos() {
        if (null != rb && null != cc) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rb.position - new Vector3(0f, (cc.height / 2f) - cc.radius, 0f), cc.radius);
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
