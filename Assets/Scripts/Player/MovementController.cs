using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float maxrspd = 10f;
    public float maxwspd = 4f;
    public float aircdelay = 0.5f;
    public float jdelay = 0.2f;
    public float bfactor = 15f;
    public float jfrc = 5f;
    public float mfrc = 50f; 
    public float groundColliderMultiplier = .75f; 
    public float groundProbeDistance = .05f; 

    Rigidbody rb;
    CapsuleCollider cc;
    InputListener il;

    bool grounded = false;
    float totime = 0f;
    float jtime = 0f;

    float defaultDrag = 0f;
    float maxspd = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultDrag = rb.drag;
        cc = GetComponent<CapsuleCollider>();

        il = GetComponent<InputListener>();
    }

    // Update is called once per frame
    void LateUpdate() {
        transform.Rotate(Vector3.up * il.GetCameraHorizontal());
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 moveDir = 
            (rb.transform.right * il.GetInputHorizontal() 
            + rb.transform.forward * il.GetInputVertical())
            .normalized * mfrc;
        maxspd = il.GetIsWalking() ? maxwspd : maxrspd; // walk/run

        bool wasGrounded = grounded;

        float radius = cc.radius * groundColliderMultiplier;
        float distance = cc.bounds.extents.y - radius + groundProbeDistance;
        RaycastHit hit;
        grounded = Physics.SphereCast(rb.position, radius, Vector3.down, out hit, distance);

        if (!grounded && wasGrounded) {
            totime = Time.time;
        }

        if (grounded) {
            rb.drag = bfactor;
        } else {
            rb.drag = defaultDrag;
        }

        bool hasAirCountrol = (Time.time - totime) <= aircdelay;
        if (grounded || hasAirCountrol) {
            if (moveDir != Vector3.zero && rb.velocity.magnitude < maxspd) {
                rb.drag = defaultDrag;
                rb.AddForce(moveDir, ForceMode.Force);
            }
        }

        bool canJump = grounded && (Time.time - jtime) > jdelay;
        if (canJump) {
            if (il.GetIsJumping()) {
                jtime = Time.time;
                rb.drag = defaultDrag;
                rb.AddForce(rb.transform.up * jfrc, ForceMode.Impulse);
            }
        }
    }
}
