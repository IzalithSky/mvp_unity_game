using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class StatusSlow : Status
{
    public float slowMagnitude = 2f;
    public NavMeshAgent agent;
    
    bool applied = false;
    float initialSpeed = 0f;

    public override void Apply() {
        if (!applied) {
            applied = true;
            initialSpeed = agent.speed;
            agent.speed /= slowMagnitude;
        }
    }

    public override void RemoveStatus() {
        agent.speed = initialSpeed;
    }
}
