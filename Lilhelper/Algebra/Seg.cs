using Lilhelper.Objs;
using UnityEngine;


namespace Lilhelper.Algebra {
    public class Seg {
        public Point from;
        public Point to;

        public float Magnitude => (to.pos - from.pos).magnitude;

        public bool IsHorizontal => from.pos.y.Approx(to.pos.y);

        public bool IsVertical => from.pos.x.Approx(to.pos.x);

        public bool IsOnSeg(Point p, float epsilon = 1e-6f) {
            return Cross(p.pos - from.pos, to.pos - from.pos).Approx(0f, epsilon);
        }

        // Try to intersect this segment with another. Returns true if they intersect and outputs the intersection point.
        // For overlapping collinear segments, returns a representative point on the overlap (the start of the overlap interval).
        public bool TryIntersect(Seg other, out Vector2 intersection, float epsilon = 1e-6f) {
            var a1 = from?.pos        ?? Vector2.zero;
            var a2 = to?.pos          ?? Vector2.zero;
            var b1 = other?.from?.pos ?? Vector2.zero;
            var b2 = other?.to?.pos   ?? Vector2.zero;
            return TryIntersect(a1, a2, b1, b2, out intersection, epsilon);
        }

        // Static utility to intersect two segments defined by their endpoints.
        public static bool TryIntersect(
            Vector2     a1,
            Vector2     a2,
            Vector2     b1,
            Vector2     b2,
            out Vector2 intersection,
            float       epsilon = 1e-6f) {
            // Based on the 2D segment intersection using cross products.
            // r = a2 - a1; s = b2 - b1
            var r    = a2 - a1;
            var s    = b2 - b1;
            var rxs  = Cross(r, s);
            var q_p  = b1 - a1;
            var qpxr = Cross(q_p, r);

            // If r x s == 0 and (q - p) x r == 0, then the segments are collinear
            if (rxs.Approx(0f, epsilon) && qpxr.Approx(0f, epsilon)) {
                // Project b1 and b2 onto r to find t parameters for a's segment
                var rr = Vector2.Dot(r, r);
                if (rr.Approx(0f, epsilon)) {
                    // a1 == a2 (degenerate segment). Check if this point lies on [b1,b2]
                    if (IsPointOnSegment(a1, b1, b2, epsilon)) {
                        intersection = a1;
                        return true;
                    }

                    intersection = default;
                    return false;
                }

                float t0 = Vector2.Dot(q_p, r) / rr;    // parameter of b1 on a
                float t1 = t0 + Vector2.Dot(s, r) / rr; // parameter of b2 on a

                // Order t0, t1
                float tMin = t0 < t1 ? t0 : t1;
                float tMax = t0 < t1 ? t1 : t0;

                // Overlap on [0,1] if tMin <= 1 and tMax >= 0
                float start = Mathf.Max(0f, tMin);
                float end   = Mathf.Min(1f, tMax);

                if (start > end + epsilon) {
                    intersection = default;
                    return false;
                }

                // If the overlap reduces to a single point (touching at an endpoint)
                if (Mathf.Abs(end - start) <= epsilon) {
                    intersection = a1 + r * ((start + end) * 0.5f);
                    return true;
                }

                // Otherwise, choose the start of the overlap interval as representative point
                intersection = a1 + r * start;
                return true;
            }

            // If r x s == 0 and (q - p) x r != 0, then the segments are parallel and non-intersecting
            if (rxs.Approx(0f, epsilon) && !qpxr.Approx(0f, epsilon)) {
                intersection = default;
                return false;
            }

            // t = (q - p) x s / (r x s)
            // u = (q - p) x r / (r x s)
            float t = Cross(q_p, s) / rxs;
            float u = Cross(q_p, r) / rxs;

            if (t >= -epsilon && t <= 1f + epsilon && u >= -epsilon && u <= 1f + epsilon) {
                // Clamp t to [0,1] to avoid slight epsilon spill
                float tc = Mathf.Clamp01(t);
                intersection = a1 + tc * r;
                return true;
            }

            intersection = default;
            return false;
        }

        private static float Cross(in Vector2 a, in Vector2 b) => a.x * b.y - a.y * b.x;

        private static bool IsPointOnSegment(in Vector2 p, in Vector2 a, in Vector2 b, float epsilon) {
            // Check collinearity via cross product and bounding via dot products
            var ap = p - a;
            var ab = b - a;
            if (!Cross(ap, ab).Approx(0f, epsilon)) return false;
            float dot = Vector2.Dot(ap, ab);
            if (dot < -epsilon) return false;
            float ab2 = Vector2.Dot(ab, ab);
            if (dot > ab2 + epsilon) return false;
            return true;
        }
    }
}
