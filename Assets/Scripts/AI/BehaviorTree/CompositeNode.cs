using System.Collections.Generic;

namespace BehaviorTree {
    public abstract class CompositeNode : Node {
        private List<Node> children = new List<Node>();

        public CompositeNode() : base() {}
        public CompositeNode(List<Node> children) {
            this.children = children;
        }

        public List<Node> GetChildren() {
            return children;
        }
    }
}
