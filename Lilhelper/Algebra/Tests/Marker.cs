namespace Lilhelper.Algebra.Tests {
    public class Marker<T> {
        private readonly T    val;
        private          bool isMarked;

        public bool IsMarked => isMarked;

        public T Val => val;

        public Marker(T val) {
            this.val = val;
        }

        public void Mark() {
            isMarked = true;
        }
    }
}
