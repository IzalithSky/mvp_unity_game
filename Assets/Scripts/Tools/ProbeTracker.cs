using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProbeTracker", menuName = "ScriptableObjects/ProbeTracker")]
public class ProbeTracker : ScriptableObject
{
    public List<Probe> probes = new List<Probe>();
}
