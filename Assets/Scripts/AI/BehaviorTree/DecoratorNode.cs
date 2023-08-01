namespace BehaviorTree {
    public abstract class DecoratorNode : Node {
        private Node child;

        public DecoratorNode() : base() {}
        public DecoratorNode(Node child) {
            this.child = child;
        }

        public Node GetChild() {
            return child;
        }
    }
}
