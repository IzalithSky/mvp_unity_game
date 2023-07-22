using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmo : MonoBehaviour, IInteractable {
    public string toolName = "";
    public int refillCount = 1;
    private Collider attachedCollider;

    private void Awake() {
        attachedCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other) {
        if (attachedCollider && attachedCollider.isTrigger) {
            HandlePickup(other.gameObject);
        }
    }

    public void Interact(GameObject interactor) {
        if (!attachedCollider || !attachedCollider.isTrigger) {
            HandlePickup(interactor);
        }
    }

    void HandlePickup(GameObject collector) {
        Tool[] tools = collector.GetComponentsInChildren<Tool>(true);
        foreach (Tool tool in tools) {
            if (tool != null && tool.toolName == toolName && tool.usesAmmo) {
                tool.ammoCount += refillCount;
                Destroy(gameObject);
                break;
            }
        }
    }

    public string GetInteractionPrompt() {
        return toolName + " +" + refillCount.ToString();
    }
}
