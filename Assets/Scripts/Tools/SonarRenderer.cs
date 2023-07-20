using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SonarRenderer : MonoBehaviour
{

    public SonarTracker sonarTracker;
    public int currentSonarIndex = 0;
    
    public MarkersState markersState;
    public bool autoScale = false;
    
    public Transform targetCamera;
    public float defaultScale = 4f;
    public float scaleMultipliyer = 1f;


    void Update() {
        if (targetCamera) {
            foreach (Marker m in markersState.AllMarkers) {
                m.transform.LookAt(targetCamera);

                if (autoScale) {
                    float distance = Vector3.Distance(m.transform.position, targetCamera.position);
                    float scale = distance * scaleMultipliyer;
                    m.transform.localScale = new Vector3(scale, scale, scale);
                } else {
                    m.transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
                }
            }
        }
    }
}
