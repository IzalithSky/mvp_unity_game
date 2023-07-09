using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public InputListener inputListener;
    public ToolHolder toolHolder;
    public List<Collider> owners;
    public Transform lookPoint;
    public Transform firePoint;
    public CameraEffects cameraEffects;
    public List<GameObject> toolExamples;

    List<Tool> tools = new List<Tool>();
    int currentToolIndex = 0;
    private bool wasNextPressedLastFrame = false;
    private bool wasPrevPressedLastFrame = false;

    void Start() {
        // Instantiate all tools in a disabled state
        foreach (var toolExample in toolExamples) {
            GameObject toolObject = Instantiate(toolExample, toolHolder.transform);
            toolObject.SetActive(false);
            
            Tool tool = toolObject.GetComponent<Tool>();
            tool.lookPoint = lookPoint;
            tool.firePoint = firePoint;
            tool.cameraEffects = cameraEffects;
            
            DamageSource damageSource = toolObject.GetComponent<DamageSource>();
            if (null != damageSource) {
                damageSource.owners = owners;
            }

            tools.Add(tool);
        }
    }

    void Update() {
        for(int i = 0; i < tools.Count; i++) {
            if (inputListener.GetIsTool(i)) {
                if (tools.Count > i) {
                    SetTool(i);
                }
            }
        }
        bool isNextPressed = inputListener.GetIsNext();
        if (isNextPressed && !wasNextPressedLastFrame) {
            SetToolNext();
        }
        wasNextPressedLastFrame = isNextPressed;

        bool isPrevPressed = inputListener.GetIsPrev();
        if (isPrevPressed && !wasPrevPressedLastFrame) {
            SetToolPrev();
        }
        wasPrevPressedLastFrame = isPrevPressed;
    }

    public void SetToolNext() {
        int nextToolIndex = (currentToolIndex + 1) % tools.Count;
        SetTool(nextToolIndex);
    }

    public void SetToolPrev() {
        int prevToolIndex = (currentToolIndex - 1 + tools.Count) % tools.Count;
        SetTool(prevToolIndex);
    }

    public void SetTool(int toolIndex) {
        currentToolIndex = toolIndex;

        // Disable current tool
        if (null != toolHolder.currentTool) {
            toolHolder.currentTool.gameObject.SetActive(false);
        }

        // Set and enable new tool
        toolHolder.currentTool = tools[toolIndex];
        toolHolder.currentTool.gameObject.SetActive(true);
    }
}
