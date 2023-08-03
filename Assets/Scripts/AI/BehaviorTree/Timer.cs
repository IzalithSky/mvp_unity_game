using UnityEngine;
using BehaviorTree;

public class Timer : Node {
    PathfindingModule pathfindingModule;
    float time;
    float duration = 3;

    public Timer(float duration) : base() {
        this.duration = duration;
    }

    protected override void OnStart() {}

    protected override NodeState OnEvaluate() {
        time += Time.deltaTime;
        if (time > duration) {
            return NodeState.FAILURE;
        }
        return NodeState.SUCCESS;
    }

    protected override void OnStop() {}

    public void Reset() {
        time = 0;
    }
}
