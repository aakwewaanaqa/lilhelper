using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lilhelper.Async;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static NUnit.Framework.Assert;
using static NUnit.Framework.Is;
using Random = UnityEngine.Random;

namespace Lilhelper.Algebra.Tests {
    public class ShapeSlice {
        private static readonly Shape shape =
            new(
                new[] {
                    new Point { pos = new Vector2(-1, +1) },
                    new Point { pos = new Vector2(+1, +1) },
                    new Point { pos = new Vector2(+1, -1) },
                    new Point { pos = new Vector2(-1, -1) }
                }
            );

        [Test]
        public void Vertical_Slice_Pass() {
            var seg = new Seg {
                from = new Point { pos = new Vector2(0, -1) },
                to   = new Point { pos = new Vector2(0, +1) }
            };

            That(shape.TrySlice(seg, out var a, out var b));
        }

        [Test]
        public void T_Slice_Pass() {
            var cutter = new Seg {
                from = shape.ByT(0.2f),
                to   = shape.ByT(0.8f),
            };

            That(shape.IsOnShape(cutter.from));
            That(shape.IsOnShape(cutter.to));
            That(shape.TrySlice(cutter, out var a, out var b));
        }

        [UnityTest]
        public IEnumerator T_Slice_Multiple_Pass() {
            var shapes = new List<Marker<Shape>> { new(shape) };
            var buffer = new List<Marker<Shape>>();
            for (int i = 0; i < 7; i++) {
                var cutter = new Seg {
                    from = shape.ByT(Random.value),
                    to   = shape.ByT(Random.value),
                };

                var alert = new OverTimeAlert(TimeSpan.FromSeconds(15));
                while (cutter.IsHorizontal || cutter.IsVertical) {
                    yield return alert.YieldWatching;
                    cutter.from = shape.ByT(Random.value);
                    cutter.to   = shape.ByT(Random.value);
                }

                foreach (var marker in shapes) {
                    if (!marker.Val.TrySlice(cutter, out var a, out var b)) continue;
                    
                    Debug.Log("SLICE!!");
                    marker.Mark();
                    buffer.Add(new(a));
                    buffer.Add(new(b));
                }

                shapes.RemoveAll(m => m.IsMarked);
                shapes.AddRange(buffer);
                buffer.Clear();
            }

            new GameObject()
               .AddComponent<GizmoHelper>()
               .SetShapes(shapes.Select(m => m.Val));
            
            yield return new WaitForSeconds(15);
        }
    }
}
