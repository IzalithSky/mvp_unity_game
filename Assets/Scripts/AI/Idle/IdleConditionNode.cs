using UnityEngine;
using BehaviorTree;

public class IdleConditionNode : Node 
{
    private MobBehaviorTree mobBT;

    public IdleConditionNode(MobBehaviorTree mobBT) 
    {
        this.mobBT = mobBT;
    }

    public override NodeState Evaluate() 
    {
        mobBT.UpdateIdleTimer(Time.deltaTime);
        
        if (mobBT.idleTimer >= mobBT.idleDuration) 
        {
            return NodeState.FAILURE;
        }

        // Additional conditions if needed...

        return NodeState.SUCCESS;
    }
}
