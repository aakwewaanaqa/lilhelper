using System.Collections.Generic;
using System.Linq;
using Unity.Hierarchy;
using UnityEngine;

namespace Lilhelper.Algebra.Tests {
    public class GizmoHelper : MonoBehaviour {

        public float                               size;
        public IEnumerable<(Color c, Shape s)>     shapes;
        public IEnumerable<(Color c, NodeGroup n)> nodes;

        public GizmoHelper SetSize(float size) {
            this.size = size;

            return this;
        }

        public GizmoHelper SetGraph(Graph graph) {
            shapes = graph.Shapes
                          .Select(s => {
                               var hsv = Random.ColorHSV();
                               hsv.g = 1f;
                               hsv.b = 1f;
                               hsv.a = 1f;
                               var rgb = Color.HSVToRGB(hsv.r, hsv.g, hsv.b);

                               return (rgb, s);
                           })
                          .ToList();

            nodes = graph.Groups
                         .Select(s => {
                              var hsv = Random.ColorHSV();
                              hsv.g = 1f;
                              hsv.b = 1f;
                              hsv.a = 1f;
                              var rgb = Color.HSVToRGB(hsv.r, hsv.g, hsv.b);

                              return (rgb, s);
                          })
                         .ToList();

            return this;
        }

        private void OnDrawGizmos() {
            foreach (var tuple in shapes) {
                Gizmos.color = tuple.c;
                Gizmos.DrawLineStrip(tuple.s.AsStrip(), true);
            }

            foreach (var tuple in nodes) {
                Gizmos.color = tuple.c;

                foreach (var node in tuple.n.nodes) {
                    Gizmos.DrawSphere(node.point.pos, node.volume * size);
                }
            }
        }

    }
}
