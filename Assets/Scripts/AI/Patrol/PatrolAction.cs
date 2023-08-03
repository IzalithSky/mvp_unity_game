using UnityEngine;
using BehaviorTree;

public class PatrolAction : Node {
    PathfindingModule pathfindingModule;

    public PatrolAction(PathfindingModule pathfindingModule) : base() {
        this.pathfindingModule = pathfindingModule;
    }

    protected override void OnStart() {}

    protected override NodeState OnEvaluate() {
        pathfindingModule.Patrol();
        return NodeState.RUNNING;
    }

    protected override void OnStop() {}
}
