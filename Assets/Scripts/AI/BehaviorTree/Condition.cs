using System;

namespace BehaviorTree {
    public class Condition : Node {
        private readonly Func<bool> predicate;

        public Condition(Func<bool> predicate) {
            this.predicate = predicate;
        }

        protected override void OnStart() {}

        protected override NodeState OnEvaluate() {
            return predicate.Invoke() ? NodeState.SUCCESS : NodeState.FAILURE;
        }

        protected override void OnStop() {}
    }
}
