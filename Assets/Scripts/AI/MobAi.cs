using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AiBehMode {
    IDLE,
    ATTACKING,
    CHASING,
    FLEEING,
    DODGING
}

public class MobAi : MonoBehaviour {
    public NavMeshAgent nm;
    public Collider player;
    public Transform toolHolder;
    public Tool tool;
    public Transform firePoint;
    public float walkRadius = 5.0f;
    public float losSearchProjectileSize = 0.5f;
    public float pathFndRadius = 1.0f;
    public float strafeDelay = 5.0f;
    public float fireingRange = 15.0f;
    public float minLosSearchRange = 0f;
    public float preAttackDelay = 0.3f;

    public AiBehMode state = AiBehMode.CHASING;
    
    bool isStrafeReady = false;
    float strafeStartTime = 0.0f;

    bool isAttackReady = false;
    float attackModeStartTime = 0.0f;

    LayerMask losSearchMask;
    

    void Start () {
        strafeStartTime = Time.time;
        
        string[] transparentLayers = new string[] { "Tools", "Projectiles", "Trigger" };
        losSearchMask = ~LayerMask.GetMask(transparentLayers); 

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0) {
            player = players[0].GetComponentInChildren<Collider>();
        }
    }

    void FixedUpdate() { 
        DoState();
        UpdateState();
    }

    void UpdateState() {
        switch (state)
        {
            case AiBehMode.CHASING:
                if (!TargetOutOfRange()) {	
                    state = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            case AiBehMode.ATTACKING:
                if (TargetOutOfRange()) {	
                    state = AiBehMode.CHASING;
                }
                if (!tool.IsReady()) {
                    state = AiBehMode.DODGING;
                }
                if (Time.time - attackModeStartTime >= preAttackDelay) {
                    isAttackReady = true;
                }
                break;
            case AiBehMode.DODGING:
                if (TargetOutOfRange()) {
                    state = AiBehMode.CHASING;
                }
                if (tool.IsReady()) {
                    state = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            default:
                break;
        }
    }

    bool TargetOutOfRange() {
        return Vector3.Distance(player.transform.position, transform.position) > (HasLineOfSight() ? fireingRange : minLosSearchRange);
    }

    void DoState() {      
        switch (state)
        {
            case AiBehMode.CHASING:
                FaceTarget(player.transform);
                nm.SetDestination(player.transform.position);
                break;
            case AiBehMode.ATTACKING:
                FaceTarget(player.transform);
                nm.SetDestination(transform.position);
                if (isAttackReady && HasLineOfSight()) {
                    isAttackReady = false;
                    attackModeStartTime = Time.time;

                    tool.Fire();
                }
                break;
            case AiBehMode.DODGING:
                FaceTarget(player.transform);
                DoStrafing();
                break;
            default:
                break;
        }
    }

    void DoStrafing() {
        if (!isStrafeReady) {
            if (Time.time - strafeStartTime >= strafeDelay) {
                isStrafeReady = true;
            }
        }

        if (isStrafeReady) {
            strafeStartTime = Time.time;
            isStrafeReady = false;

            Vector3 rndPos = transform.position 
				+ Random.insideUnitSphere * walkRadius;
				
            NavMeshHit hit;
            if (NavMesh.SamplePosition(
					rndPos, 
					out hit, 
					pathFndRadius, 
					NavMesh.AllAreas)) {
						
                nm.SetDestination(hit.position);
            }
        }
    }

    void FaceTarget(Transform t) {
        Vector3 lookPos = t.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(
								transform.rotation, 
								rotation, 
								nm.angularSpeed);

        toolHolder.LookAt(t);  
    }

    bool HasLineOfSight() {
        return HasLineOfSight(firePoint.position, player.bounds.center);
    }

    bool HasLineOfSight(Vector3 fromPosition, Vector3 toPosition) {
        Vector3 direction = toPosition - fromPosition;  
        RaycastHit hit;        
        if (Physics.SphereCast(fromPosition, losSearchProjectileSize, direction, out hit, Mathf.Infinity, losSearchMask)) {
            Debug.DrawRay(fromPosition, hit.point - fromPosition, Color.cyan);
            if (hit.collider.CompareTag("Player")) {
                return true;
            }
        }
        
        return false;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(nm.destination, 0.1f);
    }
}
