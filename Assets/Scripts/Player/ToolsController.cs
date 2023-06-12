using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public InputListener inputListener;
    public ToolHolder toolHolder;
    public Collider owner;
    public Transform lookPoint;
    public Transform firePoint;
    public List<GameObject> toolExamples;

    private List<Tool> tools = new List<Tool>();

    void Start() {
        // Instantiate all tools in a disabled state
        foreach (var toolExample in toolExamples) {
            GameObject toolObject = Instantiate(toolExample, toolHolder.transform);
            toolObject.SetActive(false);
            
            Tool tool = toolObject.GetComponent<Tool>();
            tool.lookPoint = lookPoint;
            tool.firePoint = firePoint;
            
            DamageSource damageSource = toolObject.GetComponent<DamageSource>();
            if (null != damageSource) {
                damageSource.owner = owner;
            }

            tools.Add(tool);
        }
    }

    void Update() {
        for(int i = 0; i < inputListener.toolListLength; i++) {
            if (inputListener.GetIsTool(i)) {
                if (tools.Count > i) {
                    SetTool(i);
                }
            }
        }
    }

    void SetTool(int toolIndex) {
        // Disable current tool
        if (null != toolHolder.currentTool) {
            toolHolder.currentTool.gameObject.SetActive(false);
        }

        // Set and enable new tool
        toolHolder.currentTool = tools[toolIndex];
        toolHolder.currentTool.gameObject.SetActive(true);
    }
}
