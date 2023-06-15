using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AiBehMode {
    IDLE,
    ATTACKING,
    CHASING,
    FLEEING,
    DODGING,
    GETTING_LOS
}

public class MobAi : MonoBehaviour {
    public NavMeshAgent nm;
    public Collider player;
    public Transform toolHolder;
    public Tool tool;
    public Collider bodyCollider;
    public float walkRadius = 5.0f;
    public float losSearchRadius = 5.0f;
    public float pathFndRadius = 1.0f;
    public float strafeDelay = 5.0f;
    public float fireingRange = 15.0f;
    public float preAttackDelay = 0.3f;

    AiBehMode curBehav = AiBehMode.CHASING;
    
    bool isStrafeReady = false;
    float strafeStartTime = 0.0f;

    bool isAttackReady = false;
    float attackModeStartTime = 0.0f;

    LayerMask losSearchMask;
    bool newLosPosFound = false;


    void Start () {
        strafeStartTime = Time.time;
        
        string[] transparentLayers = new string[] { "Tools", "Projectiles", "Trigger" };
        losSearchMask = ~LayerMask.GetMask(transparentLayers); 
    }

    void FixedUpdate() { 
        DoState();
        UpdateState();
        Debug.Log(newLosPosFound);
    }

    void UpdateState() {
        switch (curBehav)
        {
            case AiBehMode.CHASING:
                if (!TargetOutOfRange()) {	
                    curBehav = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            case AiBehMode.ATTACKING:
                if (!HasLineOfSight())  {
                    newLosPosFound = false;
                    curBehav = AiBehMode.GETTING_LOS;
                }
                if (TargetOutOfRange()) {	
                    curBehav = AiBehMode.CHASING;
                }
                if (!tool.IsReady()) {
                    curBehav = AiBehMode.DODGING;
                }
                if (Time.time - attackModeStartTime >= preAttackDelay) {
                    isAttackReady = true;
                }
                break;
            case AiBehMode.GETTING_LOS:
                if (HasLineOfSight())  {
                    curBehav = AiBehMode.ATTACKING;
                }
                if (TargetOutOfRange()) {
                    curBehav = AiBehMode.CHASING;
                }
                break;
            case AiBehMode.DODGING:
                if (TargetOutOfRange() && nm.remainingDistance <= nm.stoppingDistance) {
                    curBehav = AiBehMode.CHASING;
                }
                if (tool.IsReady()) {
                    curBehav = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            default:
                break;
        }
    }

    bool TargetOutOfRange() {
        return Vector3.Distance(player.transform.position, transform.position) > fireingRange;
    }

    void DoState() {      
        switch (curBehav)
        {
            case AiBehMode.CHASING:
                FaceTarget(player.transform);
                nm.SetDestination(player.transform.position);
                break;
            case AiBehMode.ATTACKING:
                FaceTarget(player.transform);
                nm.SetDestination(transform.position);
                if (isAttackReady) {
                    isAttackReady = false;
                    attackModeStartTime = Time.time;

                    tool.Fire();
                }
                break;
            case AiBehMode.GETTING_LOS:
                if (!newLosPosFound) {
                    DoLosSearch();
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

    void DoLosSearch() {
        Vector3 rndPos = transform.position 
            + Random.insideUnitSphere * losSearchRadius;
            
        NavMeshHit hit;
        if (NavMesh.SamplePosition(
                rndPos, 
                out hit, 
                pathFndRadius, 
                NavMesh.AllAreas)) {

            Vector3 lookPos = new Vector3(
                hit.position.x,
                hit.position.y + (toolHolder.position.y - bodyCollider.bounds.min.y),
                hit.position.z);
            if (HasLineOfSight(lookPos, player.transform.position)) {
                nm.SetDestination(hit.position);
                newLosPosFound = true;
            }
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
        return HasLineOfSight(toolHolder.position, player.bounds.center);
    }

    bool HasLineOfSight(Vector3 fromPosition, Vector3 toPosition) {
        Vector3 direction = toPosition - fromPosition;  
        RaycastHit hit;        
        if (Physics.Raycast(fromPosition, direction, out hit, Mathf.Infinity, losSearchMask)) {
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
