using UnityEngine;

namespace Lilhelper.Flutterive.Abstractions {
    public interface IWidget {
        bool Kill();
        RectTransform Build(RectTransform parent);
    }
}