using System;

namespace BehaviorTree {
    public class Action : Node {
        private readonly System.Action predicate;  // Use System.Action here

        public Action(System.Action predicate) {  // Use System.Action here
            this.predicate = predicate;
        }

        protected override void OnStart() {}

        protected override NodeState OnEvaluate() {
            predicate.Invoke();
            return NodeState.RUNNING;
        }

        protected override void OnStop() {}
    }
}
