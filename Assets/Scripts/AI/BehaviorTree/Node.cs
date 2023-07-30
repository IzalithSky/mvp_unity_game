using System.Collections.Generic;

namespace BehaviorTree {
    public enum NodeState {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node {

        public Node parent;

        protected NodeState state;
        protected List<Node> children;

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node() {
            this.parent = null;
        }

        public Node(List<Node> children) {
            foreach (Node child in children) {
                Attach(child);
            }
        }

        public void SetDate(string key, object value) {
            dataContext[key] = value;
        }

        public object GetData(string key) {
            object value = null;
            if (dataContext.TryGetValue(key, out value)) {
                return value;
            }

            Node node = parent;
            while (node != null) {
                value = node.GetData(key);
                if (value != null) {
                    return value;
                }
                node = node.parent;
            }

            return null;
        }

        public bool ClearData(string key) {
            if (dataContext.ContainsKey(key)) {
                dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null) {
                bool cleared = node.ClearData(key);
                if (cleared) {
                    return true;
                }
                node = node.parent;
            }

            return false;
        }

        public virtual NodeState Evaluate() {
            return NodeState.FAILURE;
        }

        private void Attach(Node node) {
            node.parent = this;
            children.Add(node);
        }
    }
}
