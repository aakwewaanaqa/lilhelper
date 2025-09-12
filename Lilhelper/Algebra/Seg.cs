using Lilhelper.Objs;
using UnityEngine;


namespace Lilhelper.Algebra {
    public class Seg {

        public readonly Point from;
        public readonly Point to;

        public Seg(Point from, Point to) {
            this.from = from;
            this.to   = to;
        }

        public float Magnitude => (to.pos - from.pos).magnitude;

        public Point MidPoint => new((from.pos + to.pos) / 2f);
        
        public bool IsOnSeg(Point p, float epsilon = 1e-6f) {
            return Cross(p.pos - from.pos, to.pos - from.pos).Approx(0f, epsilon);
        }

        /// <summary>
        /// Try to intersect this segment with another. Returns true if they intersect and outputs the intersection point.
        /// For overlapping collinear segments, returns a representative point on the overlap (the start of the overlap interval).
        /// </summary>
        public bool TryIntersect(Seg other, out Vector2 intersection, float epsilon = 1e-6f) {
            var a1 = from?.pos        ?? Vector2.zero;
            var a2 = to?.pos          ?? Vector2.zero;
            var b1 = other?.from?.pos ?? Vector2.zero;
            var b2 = other?.to?.pos   ?? Vector2.zero;

            return TryIntersect(a1, a2, b1, b2, out intersection, epsilon);
        }

        private static float Cross(in Vector2 a, in Vector2 b) => a.x * b.y - a.y * b.x;

        /// Static utility to intersect two segments defined by their endpoints.
        internal static bool TryIntersect(
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

        /// <summary>
        /// EN: Computes the shortest distance between this segment and another segment.
        /// ZH: 计算当前线段与另一条线段之间的最短距离。
        /// </summary>
        /// <param name="other">EN: The other segment. ZH: 另一条线段。</param>
        /// <param name="pa">EN: Closest point on this segment. ZH: 本线段上的最近点。</param>
        /// <param name="pb">EN: Closest point on the other segment. ZH: 另一条线段上的最近点。</param>
        /// <param name="epsilon">EN: Tolerance for floating-point comparisons. ZH: 浮点比较的容差。</param>
        /// <returns>EN: The shortest Euclidean distance. ZH: 最短欧氏距离。</returns>
        public float Distance(Seg other, out Vector2 pa, out Vector2 pb, float epsilon = Consts.EPSILON)
        {
            var a1 = this.from?.pos        ?? Vector2.zero;
            var a2 = this.to?.pos          ?? Vector2.zero;
            var b1 = other?.from?.pos      ?? Vector2.zero;
            var b2 = other?.to?.pos        ?? Vector2.zero;

            return Distance(a1, a2, b1, b2, out pa, out pb, epsilon);
        }

        /// <summary>
        /// EN: Core implementation: shortest distance between two 2D segments specified by endpoints.
        /// ZH: 核心实现：根据两条线段的端点计算它们之间的最短距离。
        /// </summary>
        /// <param name="a1">EN: Start point of segment A. ZH: 线段 A 的起点。</param>
        /// <param name="a2">EN: End point of segment A. ZH: 线段 A 的终点。</param>
        /// <param name="b1">EN: Start point of segment B. ZH: 线段 B 的起点。</param>
        /// <param name="b2">EN: End point of segment B. ZH: 线段 B 的终点。</param>
        /// <param name="pa">EN: Output closest point on segment A. ZH: 线段 A 上的最近点（输出）。</param>
        /// <param name="pb">EN: Output closest point on segment B. ZH: 线段 B 上的最近点（输出）。</param>
        /// <param name="epsilon">EN: Tolerance for float comparisons. ZH: 浮点比较的容差。</param>
        /// <returns>EN: The shortest Euclidean distance. ZH: 最短欧氏距离。</returns>
        private static float Distance(
            in Vector2 a1,
            in Vector2 a2,
            in Vector2 b1,
            in Vector2 b2,
            out Vector2 pa,
            out Vector2 pb,
            float epsilon = 1e-6f)
        {
            // If they intersect (including collinear overlap), shortest distance is zero.
            // 如果两线段相交（包括共线重叠），最短距离为 0。
            if (TryIntersect(a1, a2, b1, b2, out var inter, epsilon))
            {
                pa = inter;
                pb = inter;
                return 0f;
            }

            // Otherwise, take the minimum among the four endpoint-to-segment distances.
            // 否则，比较四种“端点到另一条线段”的距离，取其中的最小值。
            float d1 = PointToSegment(b1, a1, a2, out var p1);
            float d2 = PointToSegment(b2, a1, a2, out var p2);
            float d3 = PointToSegment(a1, b1, b2, out var p3);
            float d4 = PointToSegment(a2, b1, b2, out var p4);

            // Choose the minimum.
            // 选择最小的距离对应的两个最近点。
            pa = p1; pb = b1; float best = d1;

            if (d2 < best) { best = d2; pa = p2; pb = b2; }
            if (d3 < best) { best = d3; pa = a1; pb = p3; }
            if (d4 < best) { best = d4; pa = a2; pb = p4; }

            return best;
        }

        private static float PointToSegment(in Vector2 p, in Vector2 a, in Vector2 b, out Vector2 closest)
        {
            Vector2 ab = b - a;
            float ab2 = Vector2.Dot(ab, ab);

            if (ab2 <= Mathf.Epsilon)
            {
                // Degenerate segment (a == b)
                closest = a;
                return (p - a).magnitude;
            }

            float t = Vector2.Dot(p - a, ab) / ab2; // projection parameter along AB
            t = Mathf.Clamp01(t);                    // clamp to [0,1] for segment
            closest = a + t * ab;
            return (p - closest).magnitude;
        }

    }
}
