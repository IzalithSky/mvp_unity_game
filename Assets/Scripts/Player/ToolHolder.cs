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
        } else {
            transform.localRotation = Quaternion.Euler(-60, -45, 45);
        }
    }
}
