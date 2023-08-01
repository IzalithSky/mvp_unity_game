using System.Collections.Generic;

namespace BehaviorTree {
    public abstract class CompositeNode : Node {
        private List<Node> children = new List<Node>();

        public CompositeNode() : base() {}
        public CompositeNode(List<Node> children) : base(children) {}

        public Node GetChildren() {
            return children;
        }
    }
}
