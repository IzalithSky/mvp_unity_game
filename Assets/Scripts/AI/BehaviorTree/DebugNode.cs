namespace BehaviorTree {
    public class DebugNode : Node {
        private readonly string debugText;

        public DebugNode(string debugText) {
            this.debugText = debugText;
        }

        protected override void OnStart() {
            UnityEngine.Debug.Log(debugText);
        }

        protected override NodeState OnEvaluate() {
            return NodeState.SUCCESS;
        }

        protected override void OnStop() {}
    }
}
