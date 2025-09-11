using Lilhelper.Objs;
using UnityEngine;

namespace Lilhelper.Algebra {
    public class Point {

        public readonly float x;
        public readonly float y;

        public Vector2 pos => new(x, y);

        public bool IsZero => x.Approx(0f) && y.Approx(0f);

        public Point(Vector2 pos) {
            x = pos.x;
            y = pos.y;
        }

        public Point(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public Point Clone() {
            return new(x, y);
        }

        public static Point operator +(Point a, Point b) {
            if (b.IsZero) return a;
            if (a.IsZero) return b;

            return new(a.x + b.x, a.y + b.y);
        }

        public static Point operator -(Point a, Point b) {
            if (b.IsZero) return a;
            if (a.IsZero) return -b;

            return new(a.x - b.x, a.y - b.y);
        }

        public static Point operator -(Point a) {
            return new(-a.x, -a.y);
        }

        public float Distance(Point other) {
            return Vector2.Distance(pos, other.pos);
        }

        public bool Approx(Point other) {
            return x.Approx(other.x) && y.Approx(other.y);
        }

        public override string ToString() {
            return $"({x}, {y})";
        }

    }
}
