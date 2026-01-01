using UnityEngine;

namespace Lilhelper.Flutterive.Abstractions {
    public interface IWidget {
        GameObject Obj { get; }
        bool Kill();
        RectTransform Build(RectTransform parent);
    }
}