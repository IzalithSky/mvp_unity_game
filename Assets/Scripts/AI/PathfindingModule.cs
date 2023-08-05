using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingModule : MonoBehaviour {
    public float walkSpeed = 3.5f;
    public float runSpeed = 7.0f;

    public float pathFindRadius = 5.0f;
    public float idleMoveRadius = 5.0f;
    public float dodgeMoveRadius = 3.0f;
    public float dodgeDelay = 0.3f;
    public float idleDelay = 2.0f;

    public Transform toolHolder;
    public Tool tool;
    public Transform firePoint;

    Transform[] patrolWaypoints;
    int currentWaypointIndex = 0;
    bool isAtWaypoint = false;
    float strafeStartTime = 0;
    bool isStrafeReady = false;

    NavMeshAgent agent;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        if (!agent) {
            Debug.LogError("NavMeshAgent is not attached to the same GameObject as PathfindingModule!");
            return;
        }

        patrolWaypoints = WaypointManager.Instance.GetPatrolWaypoints();
    }

    public void Face(Transform t) {
        Vector3 lookPos = t.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(
								transform.rotation, 
								rotation, 
								agent.angularSpeed);

        toolHolder.LookAt(t);
        firePoint.LookAt(t);
    }
    
    public void DodgeMove() {
        agent.speed = runSpeed;
        DoStrafing(dodgeDelay, dodgeMoveRadius);
    }

    public void IdleMove() {
        agent.speed = walkSpeed;
        DoStrafing(idleDelay, idleMoveRadius);
    }

    void DoStrafing(float delay, float moveRadius) {
        if (!isStrafeReady) {
            if (Time.time - strafeStartTime >= delay) {
                isStrafeReady = true;
            }
        }

        if (isStrafeReady) {
            strafeStartTime = Time.time;
            isStrafeReady = false;

            Vector3 rndPos = transform.position 
				+ Random.insideUnitSphere * moveRadius;
				
            NavMeshHit hit;
            if (NavMesh.SamplePosition(
					rndPos, 
					out hit, 
					pathFindRadius, 
					NavMesh.AllAreas)) {
						
                agent.SetDestination(hit.position);
            }
        }
    }

    public void Patrol() {
        agent.speed = walkSpeed;

        if (patrolWaypoints.Length == 0) {
            return;
        }

        if (!agent.hasPath && !isAtWaypoint) {
            agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        } else if (agent.remainingDistance <= agent.stoppingDistance && !isAtWaypoint) {
            isAtWaypoint = true;
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
            agent.SetDestination(patrolWaypoints[currentWaypointIndex].position); // Start moving to the next waypoint immediately
        } else if (agent.remainingDistance > agent.stoppingDistance) {
            isAtWaypoint = false; // reset the flag when the agent starts moving
        }
    }

    public void ChaseTarget(GameObject target) {
        agent.speed = runSpeed;

        if (target) 
        {
            agent.SetDestination(target.transform.position);
        } 
    }

    public void SurroundTarget(GameObject target) {}

    public void Flee(Vector3 threatPosition) {}
}
