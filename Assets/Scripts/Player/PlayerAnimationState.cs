using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationState : MonoBehaviour
{
    public InputListener inputListener;
    public MovementController movementController;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputListener.GetInputHorizontal() != 0f || inputListener.GetInputVertical() != 0f) {
            animator.SetBool("isWalking", true);
        } else {
            animator.SetBool("isWalking", false);
        }

        if (movementController.isCrouching) {
            animator.SetBool("isCrouching", true);
        } else {
            animator.SetBool("isCrouching", false);
        }

        if (movementController.isRunning) {
            animator.SetBool("isRunning", true);
        } else {
            animator.SetBool("isRunning", false);
        }
    }
}
