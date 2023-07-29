using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

// Perception Module
public class PerceptionModule : MonoBehaviour
{
    public List<GameObject> targetsInView; // List of targets that the mob can see.
    public float viewRadius = 10.0f; // Distance within which the mob can detect targets.
    public float viewAngle = 120.0f; // The viewing angle for the mob.

    // Check if a specific target is within the line of sight.
    public bool IsTargetInView(GameObject target) {
        return false;
    }

    // Update the list of targets in the mob's view.
    public void UpdateTargetsInView() {}

    // Get the closest target within view.
    public GameObject GetClosestTarget() {
        return null;
    }
}
