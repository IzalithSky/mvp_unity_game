using UnityEngine;
using BehaviorTree;

public class PatrolActionNode : Node 
{
    private PathfindingModule pathfindingModule;

    public PatrolActionNode(PathfindingModule pathfindingModule) 
    {
        this.pathfindingModule = pathfindingModule;
    }

    public override NodeState Evaluate() 
    {
        pathfindingModule.Patrol();
        return NodeState.SUCCESS;
    }
}
