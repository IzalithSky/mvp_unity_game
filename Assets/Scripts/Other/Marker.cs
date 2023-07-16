using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public string cameraTag = "Marker Camera";
    public float defaultScale = 4f;
    public float scaleMultipliyer = 1f;
    
    private Transform cameraTransform;
    public MarkersState markersState;

    private void Awake()
    {
        markersState.AllMarkers.Add(this);
    }

    private void OnDestroy()
    {
        markersState.AllMarkers.Remove(this);
    }

    private void Update() {
        if (!cameraTransform) {
            GameObject cameraGameObject = GameObject.FindWithTag(cameraTag);

            if (cameraGameObject) {
                cameraTransform = cameraGameObject.transform;
            }
        } else {
            transform.LookAt(cameraTransform);

            if (markersState.autoScale) {
                float distance = Vector3.Distance(transform.position, cameraTransform.position);
                float scale = distance * scaleMultipliyer;
                transform.localScale = new Vector3(scale, scale, scale);
            } else {
                transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
            }
        }
    }
}

