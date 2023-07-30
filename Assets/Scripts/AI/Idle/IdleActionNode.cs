using UnityEngine;
using BehaviorTree;

public class IdleActionNode : Node 
{
    private PathfindingModule pathfindingModule;

    public IdleActionNode(PathfindingModule pathfindingModule) 
    {
        this.pathfindingModule = pathfindingModule;
    }

    public override NodeState Evaluate() 
    {
        if (Random.Range(0f, 1f) < 0.02f) 
        {
            pathfindingModule.IdleMove();
        }
        return NodeState.SUCCESS;
    }
}
