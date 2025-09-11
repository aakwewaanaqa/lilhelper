using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lilhelper.Objs;
using UnityEngine;

namespace Lilhelper.Algebra {
    public class Shape {

        private readonly IReadOnlyList<Point> points;
        private readonly IReadOnlyList<Seg>   segments;

        public int PointCount => points.Count;

        public IReadOnlyList<Point> Points => points;

        public int SegmentCount => segments.Count;

        public IReadOnlyList<Seg> Segments => segments;

        public Point this[Index i] => points[i];

        public Shape(IEnumerable<Point> points) {
            this.points = points.ToList();

            var pointList = this.points;
            var segList   = new List<Seg>();

            for (int i = 0; i < pointList.Count; i++) {
                bool isFirst = i == 0;
                bool isLast  = i == pointList.Count - 1;

                if (isFirst && isLast) {
                    segments = Array.Empty<Seg>();

                    return;
                }

                if (isLast) {
                    segList.Add(new(pointList[i], pointList[0]));
                } else {
                    segList.Add(new(pointList[i], pointList[i + 1]));
                }
            }

            segments = segList;
        }

        public bool IsOnShape(Point p, out Seg onSeg, float epsilon = Consts.EPSILON) {
            onSeg = segments.FirstOrDefault(seg => seg.IsOnSeg(p, epsilon));

            return onSeg is not null;
        }

        /// Returns a point along the closed polygonal chain by normalized arc-length t.
        /// t wraps around, so values outside [0,1] are allowed. For t==1, returns the first vertex.
        public Point ByT(float t, float epsilon = Consts.EPSILON) {
            if (points       == null || points.Count == 0) return null;
            if (points.Count == 1    || segments     == null || segments.Count == 0) return points[0];

            // Total perimeter
            float total                                    = 0f;
            for (int i = 0; i < segments.Count; i++) total += segments[i].Magnitude;

            if (total <= epsilon) return points[0];

            // Normalize t to [0,1)
            float tn = Mathf.Repeat(t, 1f);

            // Snap t very close to 1 back to 0 to avoid floating drift
            if (Mathf.Abs(tn - 1f) <= epsilon || Mathf.Abs(t - 1f) <= epsilon) return points[0];

            float target = tn * total;

            float accum = 0f;

            foreach (var seg in segments) {
                float len = seg.Magnitude;

                if (len <= epsilon) continue;

                float nextAccum = accum + len;

                // If exactly at the boundary within epsilon, return the starting or ending vertex
                if (Mathf.Abs(target - accum)     <= epsilon) return seg.from;
                if (Mathf.Abs(target - nextAccum) <= epsilon) return seg.to;

                if (target < nextAccum) {
                    float local = (target - accum) / len; // in (0,1)
                    var   p     = Vector2.Lerp(seg.from.pos, seg.to.pos, local);

                    return new(p);
                }

                accum = nextAccum;
            }

            // Fallback (should not hit because tn in [0,1))
            return points[0];
        }

        /// <summary>
        /// EN: Computes the signed area of the polygon using the shoelace formula. CCW &gt; 0, CW &lt; 0.
        /// ZH: 使用鞋带公式计算多边形的有符号面积；逆时针为正，顺时针为负。
        /// </summary>
        /// <param name="epsilon">
        /// EN: Tolerance; degenerate polygons (&lt; 3 vertices) return 0.
        /// ZH: 容差；当顶点数小于3时返回0。</param>
        /// <returns>
        /// EN: Signed area in square units.
        /// ZH: 有符号面积（平方单位）。</returns>
        public float SignedArea(float epsilon = Consts.EPSILON) {
            if (points == null || points.Count < 3) return 0f;

            double sum = 0.0;
            int    n   = points.Count;

            for (int i = 0; i < n; i++) {
                var p = points[i].pos;
                var q = points[(i + 1) % n].pos;
                sum += (double)p.x * q.y - (double)q.x * p.y;
            }

            return (float)(0.5 * sum);
        }

        /// <summary>
        /// EN: Absolute area of the polygon (2D volume). Always non-negative.
        /// ZH: 多边形的绝对面积（二维体积），始终非负。
        /// </summary>
        /// <param name="epsilon">EN: Tolerance propagated to SignedArea. ZH: 传给 SignedArea 的容差。</param>
        /// <returns>EN: Area in square units. ZH: 面积（平方单位）。</returns>
        public float Area(float epsilon = Consts.EPSILON) => Mathf.Abs(SignedArea(epsilon));

        /// <summary>
        /// EN: Volume of this shape in 2D context equals its area (alias of Area()).
        /// ZH: 在二维场景中 “体积” 等同于面积（Area 的别名）。
        /// </summary>
        public float Volume(float epsilon = Consts.EPSILON) => Area(epsilon);

        /// <summary>
        /// Try to slice this polygon by a cutter segment. Returns true if the polygon
        /// can be split into exactly two simple polygons. On success, outputs two shapes.
        /// </summary>
        public bool TrySlice(
            Seg       cutter,
            out Shape a,
            out Shape b,
            bool      sharePoints = true,
            float     epsilon     = Consts.EPSILON) {

            a = null;
            b = null;

            if (points    == null || points.Count < 3 || cutter?.from == null ||
                cutter.to == null) return false;

            var c1 = cutter.from.pos;
            var c2 = cutter.to.pos;

            return TrySlice(c1,
                c2,
                out a,
                out b,
                sharePoints,
                epsilon);
        }

        /// Overload using raw positions
        private bool TrySlice(
            Vector2   c1,
            Vector2   c2,
            out Shape a,
            out Shape b,
            bool      sharePoints = true,
            float     epsilon     = Consts.EPSILON) {

            a = null;
            b = null;
            int n = points.Count;

            if (n < 3) return false;

            // Collect intersections with polygon edges
            var insertions = new List<(int edgeIndex, float tAlongEdge, Vector2 pos)>();

            for (int i = 0; i < n; i++) {
                int j  = (i + 1) % n;
                var a1 = points[i].pos;
                var a2 = points[j].pos;

                if (Seg.TryIntersect(a1, a2, c1, c2, out var inter, epsilon)) {
                    // Filter out collinear-overlap cases (treat as not sliceable)
                    // We detect overlap when the two segments are collinear and inter lies strictly between and length of overlap > epsilon.
                    // A simple heuristic: if the intersection lies on the interior (not just endpoints) of both segments and the segments are collinear
                    // Seg.TryIntersect returns one point; we can't robustly detect broad overlap here — guard by allowing at most two unique distinct positions overall.

                    // Determine t along polygon edge using projection
                    var   e     = a2 - a1;
                    float elen2 = Vector2.Dot(e, e);
                    float t     = elen2 > epsilon ? Vector2.Dot(inter - a1, e) / elen2 : 0f;

                    // Snap to endpoints if very close
                    if (Mathf.Abs(t) <= epsilon) {
                        inter = a1;
                        t     = 0f;
                    } else if (Mathf.Abs(t - 1f) <= epsilon) {
                        inter = a2;
                        t     = 1f;
                    }

                    insertions.Add((i, Mathf.Clamp01(t), inter));
                }
            }

            if (insertions.Count < 2) return false;

            // Deduplicate intersection points by position with epsilon and keep at most two
            var unique = new List<(int edgeIndex, float tAlongEdge, Vector2 pos)>();

            for (int k = 0; k < insertions.Count; k++) {
                var  cur = insertions[k];
                bool dup = false;

                for (int m = 0; m < unique.Count; m++) {
                    if (!unique[m].pos.Approx(cur.pos, epsilon)) continue;
                    dup = true;

                    break;
                }

                if (!dup) unique.Add(cur);
            }

            if (unique.Count != 2) return false; // must be exactly two distinct intersections

            // Sort by edge index and t to maintain polygon order for insertion
            // We'll insert in increasing order of (edgeIndex, t)
            unique.Sort((x, y) => {
                int cmp = x.edgeIndex.CompareTo(y.edgeIndex);

                if (cmp != 0) return cmp;

                return x.tAlongEdge.CompareTo(y.tAlongEdge);
            });

            // Build a new list of points with intersection points inserted
            var enhanced       = new List<Point>(n + 2);
            int uIdx0          = 0;
            int nextIns        = 0;
            var insArray       = unique.ToArray();
            int enhancedIndexP = -1;
            int enhancedIndexQ = -1;

            for (int i = 0; i < n; i++) {
                // Add original vertex i
                enhanced.Add(points[i]);

                // If there is an insertion on edge (i -> i+1), add it now
                while (nextIns < insArray.Length && insArray[nextIns].edgeIndex == i) {
                    var inter = insArray[nextIns];

                    // If t == 0 we already added the vertex; skip inserting duplicate
                    if (inter.tAlongEdge > epsilon && inter.tAlongEdge < 1f - epsilon) {
                        var p = new Point(inter.pos);

                        if (enhancedIndexP      == -1) enhancedIndexP = enhanced.Count;
                        else if (enhancedIndexQ == -1) enhancedIndexQ = enhanced.Count;
                        enhanced.Add(p);
                    } else {
                        // Intersection at a vertex. Map to that existing vertex index
                        int vidx = inter.tAlongEdge <= epsilon ? enhanced.Count - 1 : (i + 1) % n;

                        if (enhancedIndexP      == -1) enhancedIndexP = vidx;
                        else if (enhancedIndexQ == -1) enhancedIndexQ = vidx;
                    }

                    nextIns++;
                }
            }

            // Ensure we captured two indices
            if (enhancedIndexP == -1 || enhancedIndexQ == -1 || enhancedIndexP == enhancedIndexQ) return false;

            // Normalize order: ensure P comes before Q in forward traversal indexes
            int P = enhancedIndexP;
            int Q = enhancedIndexQ;

            // Construct polygon A: path from P to Q (inclusive)
            var ptsA = CollectPath(enhanced, P, Q, sharePoints);
            // Construct polygon B: path from Q to P (inclusive)
            var ptsB = CollectPath(enhanced, Q, P, sharePoints);

            if (ptsA.Count < 3 || ptsB.Count < 3) return false;

            a = new(ptsA.ToArray());
            b = new(ptsB.ToArray());

            return true;

            static List<Point> CollectPath(List<Point> ring, int start, int end, bool sharePoints = true) {
                var res = new List<Point>();
                int n   = ring.Count;
                int i   = start;

                res.Add(sharePoints ? ring[i] : ring[i].Clone());

                while (i != end) {
                    i = (i + 1) % n;
                    res.Add(sharePoints ? ring[i] : ring[i].Clone());
                }

                // The Shape constructor will close the loop from last back to first, which becomes the cutter edge.
                // Remove consecutive duplicates if any
                DedupConsecutive(res);

                return res;
            }

            static void DedupConsecutive(List<Point> pts, float epsilon = Consts.EPSILON) {
                for (int i = pts.Count - 2; i >= 0; i--) {
                    if (pts[i].pos.Approx(pts[i + 1].pos, epsilon)) pts.RemoveAt(i + 1);
                }
            }
        }

        public ReadOnlySpan<Vector3> AsStrip() {
            return points.Select(p => (Vector3)p.pos).ToArray();
        }

        public override string ToString() {
            return points
                  .Select(p => p.ToString())
                  .Aggregate((a, b) => $"{a}, {b}");
        }

        // Internals used by Graph to remap close points and rebuild edges without exposing mutable collections.
        internal void RemapPointsInPlace(System.Collections.Generic.Dictionary<Point, Point> map) {
            if (points is not System.Collections.Generic.List<Point> lp) return;

            // Remap vertices
            for (int i = 0; i < lp.Count; i++) {
                var old = lp[i];

                if (old != null && map != null && map.TryGetValue(old, out var rep)) {
                    lp[i] = rep;
                }
            }

            // Rebuild segments to reference the updated vertices
            if (segments is System.Collections.Generic.List<Seg> ls) {
                ls.Clear();

                if (lp.Count <= 1) return;

                for (int i = 0; i < lp.Count; i++) {
                    bool isLast = i == lp.Count - 1;
                    var  a      = lp[i];
                    var  b      = isLast ? lp[0] : lp[i + 1];
                    ls.Add(new Seg(a, b));
                }
            }
        }

    }
}
