using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class MobAI : BehaviorTree.Tree {
    public float idleDuration = 3f;
    public float giveUpTimeout = 3f;
    
    PerceptionModule perceptionModule;
    PathfindingModule pathfindingModule;

    public bool isTimeToPatrol {get; private set;}
    public bool isPatroling {get; private set;}

    Timer idleTimer;
    Timer giveUpTimer;


    protected override Node SetupTree() {
        return new Selector(new List<Node> {
            
            new Sequence(new List<Node> {

                giveUpTimer,
                
                new Selector(new List<Node> {
                
                    new Sequence(new List<Node> {
                        new ChaseCondition(perceptionModule),
                        new DebugNode("Chasing target"),
                        new ChaseAction(perceptionModule, pathfindingModule),
                    }),
                    
                    new Sequence(new List<Node> {
                        new DebugNode("Patrolling"),
                        new PatrolAction(pathfindingModule),
                    }),
                
                }),

            }),
            
            new Sequence(new List<Node> {
                idleTimer,
                new DebugNode("Idling"),
                new IdleAction(pathfindingModule),
            }),

            new Sequence(new List<Node> {
                new DebugNode("Resetting"),
                new Action(() => {idleTimer.Reset(); giveUpTimer.Reset();}),
            }),
        
        });
    }

    protected override void OnUpdate() {}

    protected override void OnStart() {}

    void Awake() {
        perceptionModule = GetComponent<PerceptionModule>();
        pathfindingModule = GetComponent<PathfindingModule>();

        idleTimer = new Timer(idleDuration);
        giveUpTimer = new Timer(giveUpTimeout);
    }
}
