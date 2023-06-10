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
    public GameObject player;
    public Transform toolHolder;
    public Tool tool;
    public float walkRadius = 5.0f;
    public float pathFndRadius = 1.0f;
    public float strafeDelay = 5.0f;
    public float fireingRange = 15.0f;
    public float preAttackDelay = 0.3f;

    AiBehMode curBehav = AiBehMode.CHASING;
    
    bool isStrafeReady = false;
    float strafeStartTime = 0.0f;

    bool isAttackReady = false;
    float attackModeStartTime = 0.0f;


    void Start () {
        strafeStartTime = Time.time; 
    }

    void FixedUpdate() { 
        DoState();
        UpdateState();
    }

    void UpdateState() {
        switch (curBehav)
        {
            case AiBehMode.CHASING:
                if (Vector3.Distance(
					player.transform.position,
					transform.position) <= fireingRange) {
						
                    curBehav = AiBehMode.ATTACKING;
                    attackModeStartTime = Time.time;
                }
                break;
            case AiBehMode.ATTACKING:
                if (Vector3.Distance(
					player.transform.position, 
					transform.position) > fireingRange) {
						
                    curBehav = AiBehMode.CHASING;
                }
                if (!tool.IsReady()) {
                    curBehav = AiBehMode.DODGING;
                }
                if (Time.time - attackModeStartTime >= preAttackDelay) {
                    isAttackReady = true;
                }
                break;
            case AiBehMode.DODGING:
                if (nm.remainingDistance <= nm.stoppingDistance
                    && Vector3.Distance(
					    player.transform.position, 
					    transform.position) > fireingRange) {
						
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

    private void FaceTarget(Transform t) {
        Vector3 lookPos = t.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(
								transform.rotation, 
								rotation, 
								nm.angularSpeed);

        toolHolder.LookAt(t);  
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(nm.destination, 0.1f);
    }
}
