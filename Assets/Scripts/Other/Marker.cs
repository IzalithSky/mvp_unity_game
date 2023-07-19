using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public string cameraTag = "Marker Camera";
    public float defaultScale = 4f;
    public float scaleMultipliyer = 1f;
    
    private Camera c;
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
        if (!c) {
            GameObject[] cameraGameObjects = GameObject.FindGameObjectsWithTag(cameraTag);
            foreach (GameObject cameraGameObject in cameraGameObjects) {
                Camera possibleCamera = cameraGameObject.GetComponent<Camera>();
                if (possibleCamera && possibleCamera.enabled) {
                    c = possibleCamera;
                    break;
                }
            }
        }
        
        if (c) {
            transform.LookAt(c.transform);

            if (markersState.autoScale) {
                float distance = Vector3.Distance(transform.position, c.transform.position);
                float scale = distance * scaleMultipliyer;
                transform.localScale = new Vector3(scale, scale, scale);
            } else {
                transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
            }
        }
    }
}
