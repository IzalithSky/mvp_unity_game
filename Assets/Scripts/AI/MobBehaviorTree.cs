using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;

public class MobBehaviorTree : BehaviorTree.Tree 
{
    public float patrolDuration = 10f;
    public float idleDuration = 5f;
    public float chaseDuration = 5f;
    public float chaseCooldownDuration = 5f;

    public float patrolTimer = 0f;
    public float idleTimer = 0f;
    public float chaseTimer = 0f;
    public float chaseCooldownTimer = 0f;

    private PerceptionModule perceptionModule;
    private PathfindingModule pathfindingModule;
    
    private void Awake() 
    {
        perceptionModule = GetComponent<PerceptionModule>();
        pathfindingModule = GetComponent<PathfindingModule>();
    }

    protected override Node SetupTree() 
    {
        return new Selector(new List<Node> 
            {
                new Sequence(new List<Node> 
                    {
                        new PatrolConditionNode(this, perceptionModule),
                        new PatrolActionNode(pathfindingModule)
                    })
                ,new Sequence(new List<Node> 
                    {
                        new IdleConditionNode(this),
                        new IdleActionNode(pathfindingModule)
                    })
                // ,new Sequence(new List<Node> 
                //     {
                //         new ChaseConditionNode(this, perceptionModule),
                //         new ChaseActionNode(pathfindingModule, perceptionModule)
                //     })
            });
    }

    public void UpdatePatrolTimer(float deltaTime) 
    {
        patrolTimer += deltaTime;
    }

    public void UpdateIdleTimer(float deltaTime) 
    {
        idleTimer += deltaTime;
    }

    public void UpdateChaseTimer(float deltaTime) 
    {
        chaseTimer += deltaTime;
    }
}
