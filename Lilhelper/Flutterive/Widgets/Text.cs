using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {
    public struct TextParam {
        public string key;
        public State<string> data;
        public State<IWidget> child;
    }
    public class Text : IWidget {
        private readonly TextParam param;
        private GameObject self;
        public Text(in TextParam param) {
            this.param = param;
        }

        public RectTransform Build(RectTransform parent) {
            RectTransform rectTransform;
            new GameObject(nameof(Text))
                .AddCompActOut(it => it.SetParent(parent), out rectTransform)
                .AddCompAct<TextMeshProUGUI>(it => {
                    param.data.ActListen(newValue => {
                        it.text = newValue;
                        it.fontSize = 24;
                        it.color = Color.black;
                    });
                })
                .AddCompAct<ContentSizeFitter>(it => {
                    it.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    it.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                })
                .AddCompAct<VerticalLayoutGroup>(it => {
                    it.childAlignment = TextAnchor.UpperLeft;
                    it.spacing = 0;
                })
                .Out(out self);

            param.child.ActListen(newValue => {
                param.child.Value?.Kill();
                newValue.Build(rectTransform);
            });

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