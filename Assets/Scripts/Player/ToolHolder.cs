using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHolder : MonoBehaviour
{
    public InputListener inputListener;
    public Tool currentTool;
    public Transform lookPoint;
    public float defaultAimDistance = 1000f;

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(lookPoint.position, lookPoint.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            float distance = Vector3.Distance(lookPoint.position, hit.point);
            transform.LookAt(hit.point);
        }
        else
        {
            // No object is under the camera's gaze. You can either do nothing or
            // look at a point at a default distance in front of the camera.
            transform.LookAt(lookPoint.position + lookPoint.forward * defaultAimDistance);
        }

        if (inputListener.GetIsFiring() && null != currentTool) {
            currentTool.Fire();
        }
    }
}
