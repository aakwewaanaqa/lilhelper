using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {

    public struct TextParam {
        public string key;
        public State<string> data;
        public State<IWidget> child;
    }
    public class Text : WidgetBase {
        public readonly TextParam param;
        public Text(in TextParam param) {
            this.param = param;
        }

        public override RectTransform Build(RectTransform parent) {
            new GameObject(nameof(Text))
                .Out(out self)
                .AssignKey(this, param.key)
                .SetRectTransform(out RectTransform rectTransform, parent)
                .SetVerticalLayoutGroup()
                .SetContentSizeFitter()
                .AddCompAct<TextMeshProUGUI>(it => {
                    param.data?.ActListenOfHost(newValue => {
                        it.text = newValue;
                        it.fontSize = 24;
                        it.color = Color.black;
                    }, host: self);
                })
                ;

            param.child?.ActListenOfHost(newValue => {
                param.child.Value?.Kill();
                newValue?.Build(rectTransform);
            }, host: self);

            return rectTransform;
        }
    }
}