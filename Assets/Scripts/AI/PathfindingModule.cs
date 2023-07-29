using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathfindingModule : MonoBehaviour {

    public NavMeshAgent agent;
    
    // IdleMove
    public float idleMoveRadius = 5.0f;

    // Patrol
    private Transform[] patrolWaypoints;
    private int currentWaypointIndex = 0;

    private void Start() {
        patrolWaypoints = WaypointManager.Instance.GetPatrolWaypoints();
    }
    
    public void IdleMove() {
        Vector3 randomDirection = Random.insideUnitSphere * idleMoveRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, idleMoveRadius, NavMesh.AllAreas)) {
            agent.SetDestination(hit.position);
        }
    }

    public void Patrol() {
        if (patrolWaypoints.Length == 0) {
            return;
        }

        agent.SetDestination(patrolWaypoints[currentWaypointIndex].position);
        if (Vector3.Distance(transform.position, patrolWaypoints[currentWaypointIndex].position) < 1f) {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    public void MoveTo(Vector3 destination) {}

    public void ChaseTarget(GameObject target) {}

    public void SurroundTarget(GameObject target) {}

    public void Dodge(Vector3 threatPosition) {}

    public void Flee(Vector3 threatPosition) {}

    public void SetPatrolRoute(Vector3[] waypoints) {}
}
