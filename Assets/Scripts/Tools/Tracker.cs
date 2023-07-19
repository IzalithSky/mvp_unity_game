using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : Tool, ReceiverInterface
{
    public MarkersState markersState;
    public List<Sonar> sonars;
    public int currentSonarIndex = 0;
    

    public void receivePayload(GameObject payload) {
        Sonar s = payload.GetComponent<Sonar>();
        if (null != s) {
            sonars.Add(s);
        }
    }

    public void CleanUpDeadSonars()
    {
        sonars.RemoveAll(sonar => sonar == null);
    }
}
