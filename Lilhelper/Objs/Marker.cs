namespace Lilhelper.Objs {
    /// <summary>
    /// 有時候在 foreach 迴圈裡面不能更改 List 可以用這個先標記起來
    /// 最後在 foreach 外面處理
    /// </summary>
    public class Marker<T> {

        private T    val;
        private bool isMarked;

        public T Val => val;

        public bool IsMarked => isMarked;

        public Marker(T val) {
            this.val = val;
        }

        public void Mark() {
            isMarked = true;
        }

    }
}
