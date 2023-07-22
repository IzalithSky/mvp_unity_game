using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;

    public void Interact(GameObject interactor)
    {
        if (isOpen)
        {
            // Close the door
            CloseDoor();
        }
        else
        {
            // Open the door
            OpenDoor();
        }
        isOpen = !isOpen;
    }

    public string GetInteractionPrompt()
    {
        return isOpen ? "Close Door" : "Open Door";
    }

    private void OpenDoor()
    {
        // Implement door opening logic
    }

    private void CloseDoor()
    {
        // Implement door closing logic
    }
}
