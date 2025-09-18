using NUnit.Framework;
using UnityEngine;

using static NUnit.Framework.Assert;

using Lilhelper.Objs;

namespace Lilhelper.Algebra.Tests {
    public class SegIntersection {

        [Test]
        public void Vertical_Slice_Pass() {
            var a = new Seg(
                new(-1, 0),
                new(+1, 0)
            );

            var b = new Seg(
                new(0, -1),
                new(0, +1)
            );

            That(b.TryIntersect(a, out var intersection));
            That(intersection.Approx(Vector2.zero));
        }

        [Test]
        public void Collinear_Slice_Pass() {
            var a = new Seg(
                new(-1, 0),
                new(+1, 0)
            );

            var b = new Seg(
                new(-2, 0),
                new(+2, 0)
            );

            That(a.TryIntersect(b, out var intersection));
            That(intersection.Approx(new(-1, 0)));
        }

    }
}
