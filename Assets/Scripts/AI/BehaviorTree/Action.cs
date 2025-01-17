using System;

namespace BehaviorTree {
    public class Action : Node {
        private readonly System.Action predicate;

        public Action(System.Action predicate) {
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
