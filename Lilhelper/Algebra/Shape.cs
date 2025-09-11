using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Lilhelper.Algebra {
    public class Shape {
        public IReadOnlyList<Point> points;
        public IReadOnlyList<Seg>   segments;

        public Point this[Index i] => points[i];

        public float Magnitude =>
            segments
               .Select(s => s.Magnitude)
               .Aggregate((a, b) => a + b);

        public bool IsOnShape(Point p, float epsilon = 1e-6f) {
            return segments.Any(seg => seg.IsOnSeg(p, epsilon));
        }
        
        public Point ByT(float t) {
            // Returns a point along the closed polygonal chain by normalized arc-length t.
            // t wraps around, so values outside [0,1] are allowed. For t==1, returns the first vertex.
            const float epsilon = 1e-6f;
            if (points == null || points.Count == 0) return null;
            if (points.Count == 1 || segments == null || segments.Count == 0) return points[0];

            // Total perimeter
            float total = 0f;
            for (int i = 0; i < segments.Count; i++) total += segments[i].Magnitude;
            if (total <= epsilon) return points[0];

            // Normalize t to [0,1)
            float tn = Mathf.Repeat(t, 1f);
            // Snap t very close to 1 back to 0 to avoid floating drift
            if (Mathf.Abs(tn - 1f) <= epsilon || Mathf.Abs(t - 1f) <= epsilon) return points[0];

            float target = tn * total;

            float accum = 0f;
            for (int i = 0; i < segments.Count; i++) {
                var seg = segments[i];
                float len = seg.Magnitude;
                if (len <= epsilon) continue;

                float nextAccum = accum + len;

                // If exactly at the boundary within epsilon, return the starting or ending vertex
                if (Mathf.Abs(target - accum) <= epsilon) return seg.from;
                if (Mathf.Abs(target - nextAccum) <= epsilon) return seg.to;

                if (target < nextAccum) {
                    float local = (target - accum) / len; // in (0,1)
                    Vector2 p = Vector2.Lerp(seg.from.pos, seg.to.pos, local);
                    return new Point { pos = p };
                }
                accum = nextAccum;
            }

            // Fallback (should not hit because tn in [0,1))
            return points[0];
        }
        
        public Shape(Point[] points) {
            this.points = points;

            var list = new List<Seg>();
            for (int i = 0; i < points.Length; i++) {
                bool isFirst = i == 0;
                bool isLast  = i == points.Length - 1;
                if (isFirst && isLast) {
                    segments = Array.Empty<Seg>();
                    return;
                }

                if (isLast) {
                    list.Add(new Seg { from = points[i], to = points[0] });
                } else {
                    list.Add(new Seg { from = points[i], to = points[i + 1] });
                }
            }

            segments = list;
        }

        // Try to slice this polygon by a cutter segment. Returns true if the polygon
        // can be split into exactly two simple polygons. On success, outputs two shapes.
        public bool TrySlice(Seg cutter, out Shape a, out Shape b, float epsilon = 1e-6f) {
            a = null;
            b = null;
            if (points    == null || points.Count < 3 || cutter == null || cutter.from == null ||
                cutter.to == null) return false;

            Vector2 c1 = cutter.from.pos;
            Vector2 c2 = cutter.to.pos;
            return TrySlice(c1, c2, out a, out b, epsilon);
        }

        // Overload using raw positions
        public bool TrySlice(Vector2 c1, Vector2 c2, out Shape a, out Shape b, float epsilon = 1e-6f) {
            a = null;
            b = null;
            int n = points.Count;
            if (n < 3) return false;

            // Collect intersections with polygon edges
            var insertions = new List<(int edgeIndex, float tAlongEdge, Vector2 pos)>();
            for (int i = 0; i < n; i++) {
                int     j  = (i + 1) % n;
                Vector2 a1 = points[i].pos;
                Vector2 a2 = points[j].pos;

                if (Seg.TryIntersect(a1, a2, c1, c2, out var inter, epsilon)) {
                    // Filter out collinear-overlap cases (treat as not sliceable)
                    // We detect overlap when the two segments are collinear and inter lies strictly between and length of overlap > epsilon.
                    // A simple heuristic: if the intersection lies on the interior (not just endpoints) of both segments and the segments are collinear
                    // Seg.TryIntersect returns one point; we can't robustly detect broad overlap here — guard by allowing at most two unique distinct positions overall.

                    // Determine t along polygon edge using projection
                    Vector2 e     = a2 - a1;
                    float   elen2 = Vector2.Dot(e, e);
                    float   t     = elen2 > epsilon ? Vector2.Dot(inter - a1, e) / elen2 : 0f;

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
                    if (AlmostEqual(unique[m].pos, cur.pos, epsilon)) {
                        dup = true;
                        break;
                    }
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
                        var p = new Point { pos = inter.pos };
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
            var ptsA = CollectPath(enhanced, P, Q);
            // Construct polygon B: path from Q to P (inclusive)
            var ptsB = CollectPath(enhanced, Q, P);

            if (ptsA.Count < 3 || ptsB.Count < 3) return false;

            a = new Shape(ptsA.ToArray());
            b = new Shape(ptsB.ToArray());
            return true;
        }

        private static List<Point> CollectPath(List<Point> ring, int start, int end) {
            var res = new List<Point>();
            int n   = ring.Count;
            int i   = start;
            res.Add(ClonePoint(ring[i]));
            while (i != end) {
                i = (i + 1) % n;
                res.Add(ClonePoint(ring[i]));
            }

            // The Shape constructor will close the loop from last back to first, which becomes the cutter edge.
            // Remove consecutive duplicates if any
            DedupConsecutive(res);
            return res;
        }

        private static void DedupConsecutive(List<Point> pts, float epsilon = 1e-6f) {
            for (int i = pts.Count - 2; i >= 0; i--) {
                if (AlmostEqual(pts[i].pos, pts[i + 1].pos, epsilon)) pts.RemoveAt(i + 1);
            }
        }

        private static Point ClonePoint(Point p) => new() { pos = p.pos };

        private static bool AlmostEqual(Vector2 a, Vector2 b, float eps) => (a - b).sqrMagnitude <= eps * eps;

        public ReadOnlySpan<Vector3> AsStrip() {
            return points.Select(p => (Vector3)p.pos).ToArray();
        }
        
        public override string ToString() {
            return points
                  .Select(p => p.ToString())
                  .Aggregate((a, b) => $"{a}, {b}");
        }
    }
}
