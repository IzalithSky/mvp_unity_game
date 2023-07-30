using UnityEngine;
using BehaviorTree;

public class ChaseConditionNode : Node 
{
    private MobBehaviorTree mobBT;
    private PerceptionModule perceptionModule;

    public ChaseConditionNode(MobBehaviorTree mobBT, PerceptionModule perceptionModule) 
    {
        this.mobBT = mobBT;
        this.perceptionModule = perceptionModule;
    }

    public override NodeState Evaluate() 
    {
        mobBT.UpdateChaseTimer(Time.deltaTime);
        
        // Check if cooldown is active
        if (mobBT.chaseCooldownTimer > 0)
        {
            mobBT.chaseCooldownTimer -= Time.deltaTime;
            return NodeState.FAILURE; // don't chase while on cooldown
        }
        
        if (mobBT.chaseTimer >= mobBT.chaseDuration || perceptionModule.GetClosestTarget() == null) 
        {
            // Start the cooldown if chase duration is over
            if (mobBT.chaseTimer >= mobBT.chaseDuration)
            {
                mobBT.chaseCooldownTimer = mobBT.chaseCooldownDuration;
            }
            return NodeState.FAILURE;
        }

        return NodeState.SUCCESS;
    }
}
