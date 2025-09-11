using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static NUnit.Framework.Assert;
using static NUnit.Framework.Is;
using Lilhelper.Objs;

namespace Lilhelper.Algebra.Tests {
    public class SegIntersection {
        [Test]
        public void Vertical_Slice_Pass() {
            var a = new Seg {
                from = new Point { pos = new Vector2(-1, 0) },
                to   = new Point { pos = new Vector2(+1, 0) }
            };

            var b = new Seg {
                from = new Point { pos = new Vector2(0, -1) },
                to   = new Point { pos = new Vector2(0, +1) }
            };

            That(b.TryIntersect(a, out var intersection));
            That(intersection.Approx(Vector2.zero));
        }
    }
}