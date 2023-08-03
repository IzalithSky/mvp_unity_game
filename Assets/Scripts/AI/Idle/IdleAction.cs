using UnityEngine;
using BehaviorTree;

public class IdleAction : Node {
    PathfindingModule pathfindingModule ;

    public IdleAction(PathfindingModule pathfindingModule) : base() {
        this.pathfindingModule = pathfindingModule;
    }

    protected override void OnStart() {}

    protected override NodeState OnEvaluate() {
        if (Random.Range(0f, 1f) < 0.02f) {
            pathfindingModule.IdleMove();
        }
        return NodeState.RUNNING;
    }

    protected override void OnStop() {}
}
