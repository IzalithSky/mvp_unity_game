using UnityEngine;
using BehaviorTree;

public class PatrolConditionNode : Node 
{
    private MobBehaviorTree mobBT;
    private PerceptionModule perceptionModule;

    public PatrolConditionNode(MobBehaviorTree mobBT, PerceptionModule perceptionModule) 
    {
        this.mobBT = mobBT;
        this.perceptionModule = perceptionModule;
    }

    public override NodeState Evaluate() 
    {
        mobBT.UpdatePatrolTimer(Time.deltaTime);

        if (mobBT.patrolTimer >= mobBT.patrolDuration) 
        {
            return NodeState.FAILURE;
        }
        
        // Check other conditions here...
        
        return NodeState.SUCCESS;
    }
}
