using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class SlowZone : MonoBehaviour
{
    public float slowMagnitude = 2f;
    public float slowDuration = 5f;

    private void OnTriggerEnter(Collider other) {
        NavMeshAgent agent = other.GetComponentInParent<NavMeshAgent>();
        if (agent != null) {
            StatusController sc = other.GetComponentInParent<StatusController>();
            if (sc != null) {
                StatusSlow slow = other.gameObject.AddComponent<StatusSlow>();
                slow.slowMagnitude = slowMagnitude;
                slow.duration = slowDuration;
                slow.agent = agent;
                
                if (!sc.statuses.OfType<StatusSlow>().Any()) {
                    sc.ApplyStatus(slow);
                }
            }
        }
    }
}
