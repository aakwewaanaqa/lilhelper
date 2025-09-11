using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Lilhelper.Algebra {
    public class Graph {

        private readonly List<Shape>              shapes;
        private readonly List<Node>               nodes;
        private          IReadOnlyList<NodeGroup> groups;

        public IReadOnlyList<Shape> Shapes => shapes;

        public IReadOnlyList<Node> Nodes => nodes;

        public IReadOnlyList<NodeGroup> Groups => groups;

        public Graph(IEnumerable<Shape> shapes) {
            nodes       = new();
            this.shapes = shapes.ToList();
        }

        public Graph CombineCloseness(float epsilon = 1f) {
            // Build a unique list of all Point instances across all shapes
            var unique = new List<Point>();
            var index  = new Dictionary<Point, int>();

            foreach (var sh in shapes) {
                if (sh == null || sh.Count <= 0) continue;

                for (int i = 0; i < sh.Count; i++) {
                    var p = sh[new(i)];

                    if (p == null) continue;
                    if (index.ContainsKey(p)) continue; // same reference seen already
                    index[p] = unique.Count;
                    unique.Add(p);
                }
            }

            int n = unique.Count;

            if (n == 0) {
                nodes.Clear();

                return this;
            }

            // Union-Find structure
            int[] parent = new int[n];

            for (int i = 0; i < n; i++)
                parent[i] = i;

            // Merge points that are within epsilon (distance < epsilon)
            for (int i = 0; i < n; i++) {
                var pi = unique[i];
                var vi = pi.pos;

                for (int j = i + 1; j < n; j++) {
                    var pj = unique[j];
                    var vj = pj.pos;

                    if (Vector2.Distance(vi, vj) < epsilon) {
                        Union(i, j);
                    }
                }
            }

            // Build clusters and representative mapping
            var repMap     = new Dictionary<Point, Point>();
            var clusterMap = new Dictionary<int, List<Point>>();

            for (int i = 0; i < n; i++) {
                int r = Find(i);

                if (!clusterMap.TryGetValue(r, out var list)) {
                    list          = new();
                    clusterMap[r] = list;
                }

                list.Add(unique[i]);
            }

            // Clear and fill nodes
            nodes.Clear();

            foreach (var kv in clusterMap) {
                var list = kv.Value;

                if (list.Count == 0) continue;
                // Representative: use the first point to avoid moving geometry
                var rep                           = list[0];
                foreach (var p in list) repMap[p] = rep;

                nodes.Add(new() {
                    point  = rep,
                    volume = list.Count
                });
            }

            // Apply remapping to each shape (also rebuilds their segments)
            foreach (var sh in shapes) {
                sh?.RemapPointsInPlace(repMap);
            }

            return this;

            void Union(int a, int b) {
                int ra = Find(a), rb = Find(b);

                if (ra == rb) return;
                parent[rb] = ra; // simple union; no rank for simplicity
            }

            int Find(int a) {
                while (parent[a] != a) {
                    parent[a] = parent[parent[a]];
                    a         = parent[a];
                }

                return a;
            }
        }

        public Graph GroupNodes() {
            // Prepare nodes and basic arrays
            var ns = nodes;
            int n  = ns?.Count ?? 0;

            if (n <= 0) {
                groups = System.Array.Empty<NodeGroup>();

                return this;
            }

            if (n == 1) {
                groups = new List<NodeGroup> { new(new[] { ns[0] }) };

                return this;
            }

            var     pts = new Vector2[n];
            float[] wts = new float[n];

            for (int i = 0; i < n; i++) {
                pts[i] = ns[i]?.point?.pos ?? default;
                int vol = ns[i]?.volume ?? 1;
                wts[i] = vol > 0 ? vol : 1f;
            }

            // Decide k (number of groups)
            int k;

            if (n == 2) {
                var p0 = pts[0];
                var p1 = pts[1];
                k = Vector2.Distance(p0, p1) <= 1e-5f ? 1 : 2;
            } else {
                bool allSame = true;

                for (int i = 1; i < n && allSame; i++)
                    if ((pts[i] - pts[0]).sqrMagnitude > 1e-12f)
                        allSame = false;

                if (allSame) {
                    k = 1;
                } else {
                    int   maxK      = System.Math.Min(8, n);
                    float bestScore = float.NegativeInfinity;
                    int   bestK     = 1;

                    for (int kk = 2; kk <= maxK; kk++) {
                        var   centersTmp = new Vector2[kk];
                        int[] assignTmp  = new int[n];
                        KMeansWeighted(pts, wts, kk, 50, centersTmp, assignTmp);
                        float score = WeightedSilhouette(pts, wts, kk, centersTmp, assignTmp);

                        if (score > bestScore + 1e-6f || (System.Math.Abs(score - bestScore) <= 1e-6f && kk < bestK)) {
                            bestScore = score;
                            bestK     = kk;
                        }
                    }

                    k = bestK;
                }
            }

            // Final KMeans run with chosen k to get assignments
            var   centers = new Vector2[k];
            int[] assign  = new int[n];
            KMeansWeighted(pts, wts, k, 50, centers, assign);

            // Build NodeGroup buckets
            var buckets                            = new List<Node>[k];
            for (int m = 0; m < k; m++) buckets[m] = new();

            for (int i = 0; i < n; i++) {
                int g                  = assign[i];
                if (g < 0 || g >= k) g = 0; // safety
                buckets[g].Add(ns[i]);
            }

            var result = new List<NodeGroup>(k);

            for (int m = 0; m < k; m++) {
                result.Add(new(buckets[m]));
            }

            groups = result;

            return this;

            // --- Local helpers (same as in GetGroupCount) ---
            static float Dist(in Vector2 a, in Vector2 b) {
                return (a - b).magnitude;
            }

            static void KMeansWeighted(
                Vector2[] pts,
                float[]   wts,
                int       k,
                int       maxIters,
                Vector2[] centers,
                int[]     assign) {
                int n = pts.Length;
                // Initialize centers using farthest-first heuristic (deterministic)
                InitFarthestFirst(pts, wts, k, centers);

                // Initial assignment
                AssignAll(pts, centers, assign);

                for (int iter = 0; iter < maxIters; iter++) {
                    bool changed = UpdateCentersWeighted(pts, wts, k, centers, assign);

                    // Reassign after center update
                    bool anyReassign = AssignAll(pts, centers, assign);

                    if (!changed && !anyReassign) break;

                    // Handle empty clusters: if a cluster received no weight, re-seed it to farthest point
                    EnsureNoEmptyClusters(pts, wts, k, centers, assign);
                }

                return;

                static void InitFarthestFirst(Vector2[] p, float[] w, int kk, Vector2[] c) {
                    int n = p.Length;
                    // First center: pick the heaviest node
                    int   first = 0;
                    float bestW = -1f;

                    for (int i = 0; i < n; i++) {
                        if (!(w[i] > bestW)) continue;

                        bestW = w[i];
                        first = i;
                    }

                    c[0] = p[first];

                    float[] minDist2                        = new float[n];
                    for (int i = 0; i < n; i++) minDist2[i] = (p[i] - c[0]).sqrMagnitude;

                    for (int m = 1; m < kk; m++) {
                        int   idx  = 0;
                        float best = -1f;

                        for (int i = 0; i < n; i++) {
                            float d2 = minDist2[i];

                            if (!(d2 > best)) continue;

                            best = d2;
                            idx  = i;
                        }

                        c[m] = p[idx];

                        for (int i = 0; i < n; i++) {
                            float d2                          = (p[i] - c[m]).sqrMagnitude;
                            if (d2 < minDist2[i]) minDist2[i] = d2;
                        }
                    }
                }

                static bool AssignAll(Vector2[] p, Vector2[] c, int[] a) {
                    bool any = false;

                    for (int i = 0; i < p.Length; i++) {
                        int   bestIdx = 0;
                        float bestD2  = (p[i] - c[0]).sqrMagnitude;

                        for (int m = 1; m < c.Length; m++) {
                            float d2 = (p[i] - c[m]).sqrMagnitude;

                            if (!(d2 < bestD2)) continue;
                            bestD2  = d2;
                            bestIdx = m;
                        }

                        if (a[i] == bestIdx) continue;
                        a[i] = bestIdx;
                        any  = true;
                    }

                    return any;
                }

                static bool UpdateCentersWeighted(
                    Vector2[] p,
                    float[]   w,
                    int       kk,
                    Vector2[] c,
                    int[]     a) {
                    bool    changed = false;
                    var     sum     = new Vector2[kk];
                    float[] sw      = new float[kk];

                    for (int i = 0; i < p.Length; i++) {
                        int   g  = a[i];
                        float wi = w[i];
                        sum[g] += p[i] * wi;
                        sw[g]  += wi;
                    }

                    for (int m = 0; m < kk; m++) {
                        if (!(sw[m] > 0f)) continue;
                        var newC = sum[m] / sw[m];

                        if (!((newC - c[m]).sqrMagnitude > 1e-12f)) continue;
                        c[m]    = newC;
                        changed = true;
                    }

                    return changed;
                }

                static void EnsureNoEmptyClusters(
                    Vector2[] p,
                    float[]   w,
                    int       kk,
                    Vector2[] c,
                    int[]     a) {
                    float[] sw                                  = new float[kk];
                    for (int i = 0; i < p.Length; i++) sw[a[i]] += w[i];

                    for (int m = 0; m < kk; m++) {
                        if (sw[m] > 0f) continue;
                        // Re-seed empty cluster to farthest point from other centers
                        int   idx  = 0;
                        float best = -1f;

                        for (int i = 0; i < p.Length; i++) {
                            float d2 = float.PositiveInfinity;

                            for (int j = 0; j < kk; j++)
                                if (j != m) {
                                    float dd        = (p[i] - c[j]).sqrMagnitude;
                                    if (dd < d2) d2 = dd;
                                }

                            if (d2 > best) {
                                best = d2;
                                idx  = i;
                            }
                        }

                        c[m] = p[idx];
                        // Reassign that point to the empty cluster to give it mass
                        a[idx] = m;
                    }
                }
            }

            static float WeightedSilhouette(
                Vector2[] pts,
                float[]   wts,
                int       k,
                Vector2[] centers,
                int[]     assign) {
                int n = pts.Length;
                // Precompute cluster membership lists
                var clusters                            = new List<int>[k];
                for (int m = 0; m < k; m++) clusters[m] = new();
                for (int i = 0; i < n; i++) clusters[assign[i]].Add(i);

                // If any cluster is empty, very poor score
                for (int m = 0; m < k; m++)
                    if (clusters[m].Count == 0)
                        return -1f;

                float totalW                       = 0f;
                for (int i = 0; i < n; i++) totalW += wts[i];
                if (totalW        <= 0f) totalW    =  1f;

                float acc = 0f;

                for (int i = 0; i < n; i++) {
                    int gi        = assign[i];
                    var myCluster = clusters[gi];

                    // a(i): weighted mean distance to own cluster (exclude self weight)
                    float swOwn  = 0f;
                    float sumOwn = 0f;

                    foreach (int j in myCluster) {
                        if (j == i) continue;
                        float wj = wts[j];
                        sumOwn += Dist(pts[i], pts[j]) * wj;
                        swOwn  += wj;
                    }

                    float a = swOwn > 0f ? (sumOwn / swOwn) : 0f;

                    // b(i): min over other clusters of weighted mean distance
                    float b = float.PositiveInfinity;

                    for (int m = 0; m < k; m++)
                        if (m != gi) {
                            var   cl  = clusters[m];
                            float sw  = 0f;
                            float sum = 0f;

                            foreach (int j in cl) {
                                float wj = wts[j];
                                sum += Dist(pts[i], pts[j]) * wj;
                                sw  += wj;
                            }

                            if (sw <= 0f) continue;
                            float mean      = sum / sw;
                            if (mean < b) b = mean;
                        }

                    if (float.IsInfinity(b)) b = 0f;

                    float denom = System.Math.Max(a, b);
                    float s     = denom > 0f ? (b - a) / denom : 0f;

                    acc += s * wts[i];
                }

                return acc / totalW;
            }
        }

    }
}
