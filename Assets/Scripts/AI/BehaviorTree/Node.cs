namespace BehaviorTree {
    public enum NodeState {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public abstract class Node {
        protected NodeState state = NodeState.RUNNING;
        private bool started = false;

        public Node() {}

        public NodeState GetState() {
            return state;
        }

        public bool IsStarted() {
            return started;
        }

        public NodeState Evaluate() {
            if (!started) {
                OnStart();
                started = true;
            }

            state = OnEvaluate();

            if (state == NodeState.SUCCESS || state == NodeState.FAILURE) {
                started = false;
            }

            return state;
        }

        protected abstract void OnStart();
        protected abstract NodeState OnEvaluate();
        protected abstract void OnStop();
    }
}
