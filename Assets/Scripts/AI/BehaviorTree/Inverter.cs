namespace BehaviorTree {
    public class Inverter : DecoratorNode {
        public Inverter() : base() {}
        public Inverter(Node child) : base(child) {}

        protected override void OnStart() {}

        protected override NodeState OnEvaluate() {
            switch (GetChild().Evaluate()) {
                case NodeState.SUCCESS:
                    return NodeState.FAILURE;
                case NodeState.RUNNING:
                    return NodeState.RUNNING;
                case NodeState.FAILURE:
                    return NodeState.SUCCESS;
                default:
                    return NodeState.FAILURE;
            }
        }

        protected override void OnStop() {}
    }
}
