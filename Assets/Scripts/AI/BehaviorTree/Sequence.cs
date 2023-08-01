using System.Collections.Generic;

namespace BehaviorTree {
    public class Sequence : CompositeNode {
        public Sequence() : base() {}
        public Sequence(List<Node> children) : base(children) {}

        protected override void OnStart() {}
        
        protected override NodeState OnEvaluate() {
            foreach (Node child in GetChildren()) {
                switch (child.Evaluate()) {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                }
            }
            state = NodeState.SUCCESS;
            return state;
        }

        protected override void OnStop() {}
    }
}
