using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSway : MonoBehaviour {
    public float smooth = 3f;
    public float rightOffset = .3f;
    public float walkSwayIntensity = 0.0025f;
    
    Quaternion defaultRotation;
    Vector3 defaultPosition;

    void Start() {
        defaultRotation = transform.localRotation;
        defaultPosition = transform.localPosition;
    }

    void Update() {
        if (Time.timeScale > 0) {
            float inV = Input.GetAxis("Vertical");
            float inH = Input.GetAxis("Horizontal");

            float fr = -inH * rightOffset;
            float ff = -inV * rightOffset;
            
            fr = Mathf.Clamp(fr, -rightOffset, rightOffset);
            ff = Mathf.Clamp(ff, -rightOffset * .3f, rightOffset * .3f);

            Vector3 finalPosition = new Vector3(defaultPosition.x + fr, defaultPosition.y, defaultPosition.z + ff);
            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition, Time.deltaTime * rightOffset * smooth);

            if (0 != inV || 0 != inH) {
                float x = Mathf.Sin(Time.time * 6) * walkSwayIntensity;
                float y = Mathf.Cos(Time.time * 6) * walkSwayIntensity;
                transform.position = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
            }
        }
    }
}
