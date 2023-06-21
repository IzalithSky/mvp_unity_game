using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : MonoBehaviour
{
    public string toolName = "";
    public int refillCount = 1;


    private void OnTriggerEnter(Collider other) {
        Tool[] tools = other.GetComponentsInChildren<Tool>(true);
        foreach (Tool tool in tools) {
            if (tool != null && tool.toolName == toolName && tool.usesAmmo) {
                tool.ammoCount += refillCount;
                Destroy(gameObject);
                break;
            }
        }
    }
}
