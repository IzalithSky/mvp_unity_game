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
        if (inputListener.GetIsFiring() && null != currentTool) {
            currentTool.Fire();
        }
    }
}
