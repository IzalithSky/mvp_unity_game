using UnityEngine;
using BehaviorTree;

public class ChaseActionNode : Node 
{
    private PathfindingModule pathfindingModule;
    private PerceptionModule perceptionModule;

    public ChaseActionNode(PathfindingModule pathfindingModule, PerceptionModule perceptionModule) 
    {
        this.pathfindingModule = pathfindingModule;
        this.perceptionModule = perceptionModule;
    }

    public override NodeState Evaluate() 
    {
        pathfindingModule.ChaseTarget(perceptionModule.GetClosestTarget());
        return NodeState.SUCCESS;
    }
}
