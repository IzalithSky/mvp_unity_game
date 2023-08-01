using UnityEngine;

namespace BehaviorTree {
    public abstract class Tree : MonoBehaviour {
        private Node root = null;

        public Node GetRoot() {
            return root;
        }

        void Start() {
            root = SetupTree();
            OnStart();
        }

        void Update() {
            if (root != null) {
                root.Evaluate();
            }
            OnUpdate();
        }

        protected abstract Node SetupTree();
        protected abstract void OnUpdate();
        protected abstract void OnStart();
    }
}
