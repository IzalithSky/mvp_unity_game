using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingModule : MonoBehaviour {
    public float walkSpeed = 3.5f;
    public float runSpeed = 7.0f;

    public float idleMoveRadius = 5.0f;

    Transform[] patrolWaypoints;
    int currentWaypointIndex = 0;
    bool isAtWaypoint = false;

    NavMeshAgent agent;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        if (!agent) {
            Debug.LogError("NavMeshAgent is not attached to the same GameObject as PathfindingModule!");
            return;
        }

        patrolWaypoints = WaypointManager.Instance.GetPatrolWaypoints();
    }
    
    public void IdleMove() {
        agent.speed = walkSpeed;

        Vector3 randomDirection = Random.insideUnitSphere * idleMoveRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, idleMoveRadius, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
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

    public void MoveTo(Vector3 destination) {}

    public void ChaseTarget(GameObject target) {
        agent.speed = runSpeed;

        if (target) 
        {
            agent.SetDestination(target.transform.position);
        } 
    }

    public void SurroundTarget(GameObject target) {}

    public void Dodge(Vector3 threatPosition) {}

    public void Flee(Vector3 threatPosition) {}

    public void SetPatrolRoute(Vector3[] waypoints) {}
}
