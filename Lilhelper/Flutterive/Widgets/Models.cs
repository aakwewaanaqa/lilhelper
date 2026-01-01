using UnityEngine;

namespace Lilhelper.Flutterive.Widgets {
    public interface ISizing { }
    public class FitContent : ISizing { }
    public class Inherit : ISizing { }
    public class Size : ISizing {
        public Vector2 size { get; private set; }
        public Size(Vector2 size = default) {
            this.size = size;
        }
        public Size(float width = 0, float height = 0) {
            this.size = new Vector2(width, height);
        }
    }
}