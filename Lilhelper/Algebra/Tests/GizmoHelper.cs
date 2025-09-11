using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lilhelper.Algebra.Tests {
    public class GizmoHelper : MonoBehaviour {
        private IEnumerable<(Color c, Shape s)> tuples;

        public GizmoHelper SetShapes(IEnumerable<Shape> shapes) {
            tuples = shapes
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
            foreach (var tuple in tuples) {
                Gizmos.color = tuple.c;
                Gizmos.DrawLineStrip(tuple.s.AsStrip(), true);
            }
        }
    }
}
