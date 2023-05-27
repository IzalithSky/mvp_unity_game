using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint; // Starting position of the platform
    public Transform endPoint; // Ending position of the platform
    public float speed = 2f; // Speed at which the platform moves

    private Vector3 targetPosition; // Current target position
    private bool movingToEnd = true; // Flag to determine direction of movement

    private void Start()
    {
        // Set the initial target position to the end point
        targetPosition = endPoint.position;
    }

    private void FixedUpdate()
    {
        // Move the platform towards the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Check if the platform has reached the target position
        if (transform.position == targetPosition)
        {
            // Toggle the direction of movement
            movingToEnd = !movingToEnd;

            // Update the target position based on the direction of movement
            targetPosition = movingToEnd ? endPoint.position : startPoint.position;
        }
    }
}
