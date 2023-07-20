using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour {
    public ProbeTracker probeTracker;
    public float distance = -1f;
    

    private void Update() {
        if (probeTracker.targetGameObject) {
            distance = Vector3.Distance(transform.position, probeTracker.targetGameObject.transform.position);
        }
    }

    private void Awake() {
        probeTracker.probes.Add(this);
    }

    private void OnDestroy() {
        probeTracker.probes.Remove(this);
    }
}
