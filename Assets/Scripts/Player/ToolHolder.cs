using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHolder : MonoBehaviour
{
    public InputListener inputListener;
    public Tool currentTool;

    // Update is called once per frame
    void Update()
    {
        if (inputListener.GetIsWalking()) {
            if (inputListener.GetIsFiring() && null != currentTool) {
                currentTool.Fire();
            }
        } else if (currentTool != null && currentTool.toolName != "Scanner") {
            transform.localRotation = Quaternion.Euler(-60, -45, 45);
            transform.localPosition = new Vector3(0f, -0.4f, 0.3f);
        }
    }
}
