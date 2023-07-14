using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public ToolHolder toolHolder;
    public List<Collider> owners;
    public Transform lookPoint;
    public Transform firePoint;
    public CameraEffects cameraEffects;
    public List<GameObject> toolExamples;

    List<Tool> tools = new List<Tool>();
    int currentToolIndex = 0;

    PlayerControls playerControls;


    void Start() {
        // Instantiate all tools in a disabled state
        foreach (var toolExample in toolExamples) {
            if (toolExample) {
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
    }

    void Awake()
    {
        playerControls = new PlayerControls();

        playerControls.Tools.ToolPrev.performed += ctx => SetToolPrev();
        playerControls.Tools.ToolNext.performed += ctx => SetToolNext();

        playerControls.Tools.Tool0.performed += ctx => SetTool(0);
        playerControls.Tools.Tool1.performed += ctx => SetTool(1);
        playerControls.Tools.Tool2.performed += ctx => SetTool(2);
        playerControls.Tools.Tool3.performed += ctx => SetTool(3);
        playerControls.Tools.Tool4.performed += ctx => SetTool(4);
        playerControls.Tools.Tool5.performed += ctx => SetTool(5);
        playerControls.Tools.Tool6.performed += ctx => SetTool(6);
        playerControls.Tools.Tool7.performed += ctx => SetTool(7);
        playerControls.Tools.Tool8.performed += ctx => SetTool(8);
        playerControls.Tools.Tool9.performed += ctx => SetTool(9);
        playerControls.Tools.Tool10.performed += ctx => SetTool(10);

        playerControls.Enable();
    }

    void OnDestroy()
    {
        playerControls.Dispose();
    }

    void SetToolNext() {
        int nextToolIndex = (currentToolIndex + 1) % tools.Count;
        SetTool(nextToolIndex);
    }

    void SetToolPrev() {
        int prevToolIndex = (currentToolIndex - 1 + tools.Count) % tools.Count;
        SetTool(prevToolIndex);
    }

    void SetTool(int toolIndex) {
        currentToolIndex = toolIndex;

        if (toolIndex >= 0 && toolIndex < tools.Count && tools[toolIndex]) {
            if (toolHolder.currentTool) {
                toolHolder.currentTool.gameObject.SetActive(false);
            }

            toolHolder.currentTool = tools[toolIndex];
            toolHolder.currentTool.gameObject.SetActive(true);
        }
    }
}
