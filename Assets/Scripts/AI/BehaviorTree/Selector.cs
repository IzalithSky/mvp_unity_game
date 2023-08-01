using System.Collections.Generic;

namespace BehaviorTree {
    public class Selector : CompositeNode {
        public Selector() : base() {}
        public Selector(List<Node> children) : base(children) {}
        
        protected override void OnStart() {}
        
        protected override NodeState OnEvaluate() {
            foreach (Node child in GetChildren()) {
                switch (child.Evaluate()) {
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS; 
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    case NodeState.FAILURE:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }

        protected override void OnStop() {}
    }
}
