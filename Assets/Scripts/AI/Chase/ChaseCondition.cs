using UnityEngine;
using BehaviorTree;

public class ChaseCondition : Node {
    PerceptionModule perceptionModule;

    public ChaseCondition(PerceptionModule perceptionModule) : base() {
        this.perceptionModule = perceptionModule;
    }

    protected override void OnStart() {}

    protected override NodeState OnEvaluate() {
        if (perceptionModule.GetClosestTarget()) {
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }

    protected override void OnStop() {}
}
