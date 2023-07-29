using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance;
    public Transform[] globalPatrolWaypoints;

    List<Transform> navMeshWaypoints;
    
    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }

        FilterNavMeshWaypoints();
    }

    void FilterNavMeshWaypoints() {
        navMeshWaypoints = new List<Transform>();
        
        foreach (var waypoint in globalPatrolWaypoints) {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(waypoint.position, out hit, 1.0f, NavMesh.AllAreas)) {
                navMeshWaypoints.Add(waypoint);
            }
        }
    }

    public Transform[] GetPatrolWaypoints() {
        return navMeshWaypoints.ToArray();
    }
}
