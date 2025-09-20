namespace Lilhelper.Objs {
    public class Late<T> {
        public T val;
        
        public static implicit operator T(Late<T> self) => self.val;
    }
}
