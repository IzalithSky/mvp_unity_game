using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Transform firePoint;
    public float interactionDistance = 2f;
    public LayerMask interactionLayer;
    
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();

        // Register the interaction event.
        controls.Menus.Interact.performed += _ => Interact();
    }

    void Interact()
    {
        Ray ray = new Ray(firePoint.position, firePoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject);
            }
        }
    }

    private void OnEnable()
    {
        controls.Menus.Enable();
    }

    private void OnDisable()
    {
        controls.Menus.Disable();
    }
}
