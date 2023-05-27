using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public InputListener inputListener;
    public ToolHolder toolHolder;
    public GameObject owner;
    public Transform lookPoint;
    public List<GameObject> toolExamples;

    void Update() {
        for(int i = 0; i < inputListener.toolListLength; i++) {
            if (inputListener.GetIsTool(i)) {
                if (toolExamples.Count > i) {
                    SetTool(i);
                }
            }
        }
    }

    void SetTool(int toolIndex) {
        if (null != toolHolder.currentTool) {
            Destroy(toolHolder.currentTool.gameObject);
        }

        GameObject toolObject = Instantiate(toolExamples[toolIndex], toolHolder.transform);
        Tool tool = toolObject.GetComponent<Tool>();

        tool.lookPoint = lookPoint;
        tool.owner = owner;
        toolHolder.currentTool = tool;
    }
}
