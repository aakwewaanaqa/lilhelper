using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Lilhelper.Async;
using Lilhelper.Objs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static NUnit.Framework.Assert;
using static NUnit.Framework.Is;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Lilhelper.Algebra.Tests {
    public class ShapeSlice {

        private static readonly Shape rect =
            new(
                new[] {
                    new Point(-1, +1),
                    new Point(+1, +1),
                    new Point(+1, -1),
                    new Point(-1, -1)
                }
            );

        [Test]
        public void Vertical_Slice_Pass() {
            var seg = new Seg(
                new(0, -1),
                new(0, +1)
            );

            That(rect.TrySlice(seg, out var a, out var b));
        }

        [Test]
        public void T_Slice_Pass() {
            var cutter = new Seg(
                rect.ByT(0.2f),
                rect.ByT(0.8f)
            );

            That(rect.IsOnShape(cutter.from, out _));
            That(rect.IsOnShape(cutter.to,   out _));
            That(rect.TrySlice(cutter, out var a, out var b));
        }

        [UnityTest]
        public IEnumerator T_Slice_Multiple_Pass() {
            var shapes = new List<Marker<Shape>> { new(rect) };
            var buffer = new List<Marker<Shape>>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 7; i++) {
                Seg cutter = null;

                var alert = new OverTimeAlert(TimeSpan.FromSeconds(15));

                for (;;) {
                    float ta = Random.value;
                    float tb = Random.value;

                    cutter = new(
                        rect.ByT(ta),
                        rect.ByT(tb)
                    );

                    rect.IsOnShape(cutter.from, out var segA);
                    rect.IsOnShape(cutter.to,   out var segB);

                    if (ReferenceEquals(segA, segB)) {
                        yield return alert.YieldWatching;

                        continue;
                    }


                    yield return alert.YieldWatching;

                    break;
                }

                foreach (var marker in shapes) {
                    if (!marker.Val.TrySlice(cutter, out var a, out var b)) continue;

                    marker.Mark();
                    buffer.Add(new(a));
                    buffer.Add(new(b));
                }

                shapes.RemoveAll(m => m.IsMarked);
                shapes.AddRange(buffer);
                buffer.Clear();
            }

            Debug.Log(stopwatch.Elapsed);

            var graph = new Graph(shapes.Select(m => m.Val))
                       .CombineCloseness(0.06f)
                       .GroupNodes();

            Debug.Log(stopwatch.Elapsed);
            stopwatch.Stop();

            new GameObject()
               .EnsureCompAct<GizmoHelper>(
                    it => it.SetGraph(graph).SetSize(0.02f),
                    out var helper);

            That(helper.nodes,  GreaterThan(0));
            That(helper.shapes, GreaterThan(0));

            yield return new WaitForSeconds(15);
        }

    }
}
