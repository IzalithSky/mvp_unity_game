using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class MobAI : BehaviorTree.Tree {
    public float idleDuration = 3f;
    public float patrolDuration = 3f;
    public float giveUpTimeout = 3f;
    
    PerceptionModule perceptionModule;
    PathfindingModule pathfindingModule;

    public bool isTimeToPatrol {get; private set;}
    public bool isPatroling {get; private set;}

    Timer patrolTimer;
    Timer idleTimer;
    Timer giveUpTimer;


    protected override Node SetupTree() {
        return new Selector(new List<Node> {
            
            new Sequence(new List<Node> {
                new ChaseCondition(perceptionModule),
                giveUpTimer,
                new DebugNode("Chasing target"),
                new ChaseAction(perceptionModule, pathfindingModule),
            }),
            
            new Sequence(new List<Node> {
                patrolTimer,
                new DebugNode("Patrolling"),
                new PatrolAction(pathfindingModule),
            }),
            
            new Sequence(new List<Node> {
                idleTimer,
                new DebugNode("Idling"),
                new IdleAction(pathfindingModule),
            }),

        });
    }

    protected override void OnUpdate() {
        if (idleTimer.GetState() == NodeState.FAILURE) {
            idleTimer.Reset();
            if (patrolTimer.GetState() == NodeState.FAILURE) {
                patrolTimer.Reset();
            }
            if (giveUpTimer.GetState() == NodeState.FAILURE) {
                giveUpTimer.Reset();
            }
        }
    }

    protected override void OnStart() {}

    void Awake() {
        perceptionModule = GetComponent<PerceptionModule>();
        pathfindingModule = GetComponent<PathfindingModule>();

        patrolTimer = new Timer(patrolDuration);
        idleTimer = new Timer(idleDuration);
        giveUpTimer = new Timer(giveUpTimeout);
    }
}
