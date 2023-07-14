using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker : MonoBehaviour
{
    public string cameraTag = "Marker Camera";
    private Transform cameraTransform;

    private void Update() {
        if (!cameraTransform) {
            GameObject cameraGameObject = GameObject.FindWithTag(cameraTag);

            if (cameraGameObject) {
                cameraTransform = cameraGameObject.transform;
            }
        } else {
            transform.LookAt(cameraTransform);
        }
    }
}