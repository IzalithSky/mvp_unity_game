using System;

namespace BehaviorTree {
    public class ActionInstant : Node {
        private readonly System.Action predicate;

        public ActionInstant(System.Action predicate) {
            this.predicate = predicate;
        }

        protected override void OnStart() {}

        protected override NodeState OnEvaluate() {
            predicate.Invoke();
            return NodeState.SUCCESS;
        }

        protected override void OnStop() {}
    }
}
