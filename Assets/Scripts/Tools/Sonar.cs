using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    public SonarTracker sonarTracker;

    private void Awake()
    {
        sonarTracker.sonars.Add(this);
    }

    private void OnDestroy()
    {
        sonarTracker.sonars.Remove(this);
    }
}