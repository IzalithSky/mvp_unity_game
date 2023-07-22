using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public MarkersState markersState;

    public float defaultScale = 4f;
    public float scaleMultipliyer = 1f;

    private void Awake()
    {
        markersState.AllMarkers.Add(this);
    }

    private void OnDestroy()
    {
        markersState.AllMarkers.Remove(this);
    }

    void Update() {
        if (markersState.targetCamera) {
            transform.LookAt(markersState.targetCamera);

            if (markersState.autoScale) {
                float distance = Vector3.Distance(transform.position, markersState.targetCamera.position);
                float scale = distance * scaleMultipliyer;
                transform.localScale = new Vector3(scale, scale, scale);
            } else {
                transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
            }
        }
    }
}
