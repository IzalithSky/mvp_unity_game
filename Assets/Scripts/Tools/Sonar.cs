using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        cam.enabled = false;
    }

    public void SetCamEnabled(bool flag) {
        cam.enabled = flag;
    }
}
