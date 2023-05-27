using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVerticalRotation : MonoBehaviour
{
    public InputListener inputListener;
    public float minAngle = -90f;
    public float maxAngle = 90f;

    float yRotate = 0.0f;

    // Update is called once per frame
    void LateUpdate() {
        yRotate -= inputListener.GetCameraVertical();
        yRotate = ClampAngle(yRotate, minAngle, maxAngle);
        transform.localRotation = Quaternion.Euler(yRotate, 0.0f, 0.0f);
    }

    float ClampAngle(float lfAngle, float lfMin, float lfMax) {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
