using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class MobAI : BehaviorTree.Tree {
    public float idleDuration = 3f;
    public float patrolDuration = 3f;
    
    PathfindingModule pathfindingModule;
    public bool isTimeToPatrol {get; private set;}
    public bool isPatroling {get; private set;}

    Timer patrolTimer;
    Timer idleTimer;

    protected override Node SetupTree() {
        return new Selector(new List<Node> {
            new Sequence(new List<Node> {
                patrolTimer,
                new PatrolAction(pathfindingModule),
            }),
            new Sequence(new List<Node> {
                idleTimer,
                new IdleAction(pathfindingModule),
            }),
        });
    }

    protected override void OnUpdate() {
        if (patrolTimer.GetState() == NodeState.FAILURE && idleTimer.GetState() == NodeState.FAILURE) {
            patrolTimer.Reset();
            idleTimer.Reset();
        }
    }

    protected override void OnStart() {}

    void Awake() {
        pathfindingModule = GetComponent<PathfindingModule>();
        patrolTimer = new Timer(patrolDuration);
        patrolTimer.Reset();
        idleTimer = new Timer(idleDuration);
        idleTimer.Reset();
    }
}
