using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {

    public struct EdgeInsets {
        public int left;
        public int right;
        public int top;
        public int bottom;
    }

    /// <summary>
    /// Margin 所需的參數集合。
    /// </summary>
    public struct MarginParam {
        /// <summary>識別用 key</summary>
        public string key;
        public State<EdgeInsets> edge;
        /// <summary>包含的子 Widget（可觀察，變更時會建立新子物件）</summary>
        public State<IWidget> child;
    }

    /// <summary>
    /// Margin：在建立的子容器周圍加上空白 (offset) 的包覆元件。
    /// 實作上建立一個外層容器作為 Layout 的一員，並在其下建立內層容器 (inner)
    ///，再將子 Widget 建立在內層容器上，透過設定 `offsetMin` / `offsetMax` 實現 margin。
    /// </summary>
    public class Margin : IWidget {
        public readonly MarginParam param;
        private GameObject self;

        public Margin(in MarginParam param) {
            this.param = param;
        }

        public RectTransform Build(RectTransform parent) {
            new GameObject(nameof(Margin))
                .Out(out self)
                .SetUpLayout(parent, out RectTransform rectTransform, out VerticalLayoutGroup layoutGroup)
                ;

            param.edge.ActListenOfHost(edge => {
                layoutGroup.padding.left = edge.left;
                layoutGroup.padding.right = edge.right;
                layoutGroup.padding.top = edge.top;
                layoutGroup.padding.bottom = edge.bottom;
            }, host: self);

            // 當 child 改變時，銷毀舊 child 並建立新的 child 在 innerRect
            param.child?.ActListenOfHost(newValue => {
                param.child.Value?.Kill();
                newValue?.Build(rectTransform);
            }, host: self);

            return rectTransform;
        }

        public bool Kill() {
            if (self.DoExists()) {
                Object.Destroy(self);
                return true;
            }
            return false;
        }
    }
}