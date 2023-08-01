using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class MobAI : BehaviorTree.Tree {
    public float patrolDelay = 3f;
    public float patrolDuration = 3f;
    
    PathfindingModule pathfindingModule;
    public bool isTimeToPatrol {get; private set;}
    public bool isPatroling {get; private set;}

    protected override Node SetupTree() {
        return new Selector(new List<Node>{
            new Sequence(new List<Node>{
                new PatrolCondition(this),
                new PatrolAction(pathfindingModule)
            }),
            new Sequence(new List<Node>{
                new ManageStatesAction(this),
                new IdleAction(pathfindingModule)
            })
        });
    }

    protected override void OnUpdate() {}

    protected override void OnStart() {}

    void Awake() {
        pathfindingModule = GetComponent<PathfindingModule>();
        isTimeToPatrol = false;
        isPatroling = true;
    }

    public void StartTimerIsTimeToPatrol() {
        Invoke("SetIsTimeToPatrol", patrolDelay);
    }

    void SetIsTimeToPatrol() {
        isTimeToPatrol = true;
    }

    void ResetIsTimeToPatrol() {
        isTimeToPatrol = false;
    }

    public void StartTimerIsPatroling() {
        Invoke("SetIsPatroling", patrolDuration);
    }

    void SetIsPatroling() {
        isPatroling = true;
    }

    void ResetIsPatroling() {
        isPatroling = false;
    }
}
