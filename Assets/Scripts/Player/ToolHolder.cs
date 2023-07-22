using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHolder : MonoBehaviour
{
    public InputListener inputListener;
    public Tool currentTool;
    public Transform lookPoint;
    public float defaultAimDistance = 1000f;
    public float distance = 0f;

    public List<string> nonLookingTools;

    LayerMask mask;

    void Start() {
        string[] transparentLayers = new string[] {"Tools", "Projectiles", "Trigger", "Smoke", "Player"};
        mask = ~LayerMask.GetMask(transparentLayers); 
    }

    void Update() {
        RaycastHit hit;
        if (Physics.Raycast(lookPoint.position, lookPoint.forward, out hit, Mathf.Infinity, mask)) {
            distance = Vector3.Distance(lookPoint.position, hit.point);
            
            if (currentTool != null && !nonLookingTools.Contains(currentTool.toolName)) {
                transform.LookAt(hit.point);
            } else {
                transform.LookAt(lookPoint.position + lookPoint.forward * defaultAimDistance);
            }
        } else {
            distance = -1f;
            transform.LookAt(lookPoint.position + lookPoint.forward * defaultAimDistance);
        }

        if (inputListener.GetIsFiring() && currentTool != null) {
            currentTool.Fire();
        }

        if (inputListener.GetIssSwitching() && currentTool != null) {
            currentTool.Switch();
        }
    }
}
