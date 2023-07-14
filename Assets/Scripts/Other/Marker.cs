using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public string cameraTag = "Marker Camera";
    public bool autoScale = false;    
    public float originalSize = 1f; // The original size of the Marker
    public float mult = 1.14f;
    
    private Transform cameraTransform;

    private void Update() {
        if (!cameraTransform) {
            GameObject cameraGameObject = GameObject.FindWithTag(cameraTag);

            if (cameraGameObject) {
                cameraTransform = cameraGameObject.transform;
            }
        } else {
            transform.LookAt(cameraTransform);

            if (autoScale) {
                float distance = Vector3.Distance(transform.position, cameraTransform.position);
                // float perceivedSize = 2 * Mathf.Atan(originalSize / (2 * distance)) * Mathf.Rad2Deg;
                // float scale = perceivedSize / originalSize;
                float scale = distance * mult;
                transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
