using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class MobAI : BehaviorTree.Tree {
    public float idleDuration = 3f;
    public float giveUpTimeout = 3f;
    public float preAttackDelay = 0.5f;
    public float attackRange = 1f;
    public float combatRange = 5f;
    
    PerceptionModule perceptionModule;
    PathfindingModule pathfindingModule;
    Tool tool;

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
                        new Condition(() => !tool.IsReady()),
                        new Condition(() => Vector3.Distance(
                            perceptionModule.GetClosestTarget().transform.position, 
                            transform.position) <= combatRange),
                        new ActionInstant(() => pathfindingModule.Face(perceptionModule.GetClosestTarget().transform)),
                        new DebugNode("Dodging"),
                        new ActionInstant(() => pathfindingModule.DodgeMove()),
                    }),

                    new Sequence(new List<Node> {
                        new Condition(() => perceptionModule.GetClosestTarget() != null),
                        new Condition(() => Vector3.Distance(
                            perceptionModule.GetClosestTarget().transform.position, 
                            transform.position) <= attackRange),
                        new ActionInstant(() => pathfindingModule.Face(perceptionModule.GetClosestTarget().transform)),
                        new Delay(preAttackDelay),
                        new DebugNode("Attacking target"),
                        new ActionInstant(() => tool.Fire()),
                    }),
                
                    new Sequence(new List<Node> {
                        new Condition(() => perceptionModule.GetClosestTarget() != null),
                        new DebugNode("Chasing target"),
                        new Action(() => pathfindingModule.ChaseTarget(perceptionModule.GetClosestTarget())),
                    }),
                    
                    new Sequence(new List<Node> {
                        new DebugNode("Patrolling"),
                        new Action(() => pathfindingModule.Patrol()),
                    }),
                
                }),

            }),
            
            new Sequence(new List<Node> {
                idleTimer,
                new DebugNode("Idling"),
                new ActionInstant(() => pathfindingModule.IdleMove()),
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
        tool = GetComponentInChildren<Tool>();

        idleTimer = new Timer(idleDuration);
        giveUpTimer = new Timer(giveUpTimeout);
    }
}
