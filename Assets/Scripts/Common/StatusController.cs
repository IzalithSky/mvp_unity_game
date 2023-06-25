using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    public List<Status> statuses;

    private void Start()
    {
        statuses = new List<Status>();
    }

    public void ApplyStatus(Status status)
    {
        statuses.Add(status);
    }

    private void FixedUpdate()
    {
        for (int i = statuses.Count - 1; i >= 0; i--)
        {
            Status status = statuses[i];
            if (!status.IsActive())
            {
                status.RemoveStatus();
                statuses.RemoveAt(i);
                Destroy(status);
            }
        }
    }
}