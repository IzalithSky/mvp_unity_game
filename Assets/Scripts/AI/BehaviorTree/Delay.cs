using UnityEngine;
using BehaviorTree;

public class Delay : Node {
    PathfindingModule pathfindingModule;
    float startTime;
    float duration = 3;

    public Delay(float duration) : base() {
        this.duration = duration;
    }

    protected override void OnStart() {
        startTime = Time.time;
    }

    protected override NodeState OnEvaluate() {
        if (Time.time - startTime > duration) {
            return NodeState.SUCCESS;
        }
        
        return NodeState.RUNNING;
    }

    protected override void OnStop() {}
}
