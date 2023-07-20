using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : MonoBehaviour
{
    public ProbeTracker probeTracker;

    private void Awake()
    {
        probeTracker.probes.Add(this);
    }

    private void OnDestroy()
    {
        probeTracker.probes.Remove(this);
    }
}
