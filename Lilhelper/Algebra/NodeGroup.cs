using System.Collections.Generic;

namespace Lilhelper.Algebra {
    public class NodeGroup {

        public IEnumerable<Node> nodes;

        public IEnumerable<Node> Nodes => nodes;

        public NodeGroup(IEnumerable<Node> nodes) {
            this.nodes = nodes;
        }
    }
}
