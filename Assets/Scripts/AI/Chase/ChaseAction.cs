using UnityEngine;
using BehaviorTree;

public class ChaseAction : Node {
    PerceptionModule perceptionModule;
    PathfindingModule pathfindingModule;

    public ChaseAction(PerceptionModule perceptionModule, PathfindingModule pathfindingModule) : base() {
        this.perceptionModule = perceptionModule;
        this.pathfindingModule = pathfindingModule;
    }

    protected override void OnStart() {}

    protected override NodeState OnEvaluate() {
        pathfindingModule.ChaseTarget(perceptionModule.GetClosestTarget());
        return NodeState.RUNNING;
    }

    protected override void OnStop() {}
}
