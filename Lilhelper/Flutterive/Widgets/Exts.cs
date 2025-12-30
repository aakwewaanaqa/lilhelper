using JetBrains.Annotations;

using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {
    public static class Exts {
        internal static GameObject SetUpLayout(
            this GameObject go,
            RectTransform parent,
            out RectTransform rectTransform,
            out VerticalLayoutGroup layoutGroup) {
            bool isRoot = go.GetComponent<Canvas>();
            return go
                .EnsureCompActOut(it => it.SetParent(parent, false), out rectTransform)
                .AddCompActOut(it => {
                    it.childAlignment = TextAnchor.UpperLeft;
                    it.childControlHeight = false;
                    it.childControlWidth = false;
                    it.childForceExpandHeight = false;
                    it.childForceExpandWidth = false;
                    it.spacing = 0;
                }, out layoutGroup).Let((go) => isRoot
                    ? go
                    : go.AddCompAct<ContentSizeFitter>(it => {
                        it.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                        it.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                    }));
        }

        public static EdgeInsets EdgeAll(this int value) {
            return new EdgeInsets {
                left = value,
                right = value,
                top = value,
                bottom = value,
            };
        }

        public static IWidget Text(this string data) {
            return new Text(
                new TextParam {
                    data = data
                }
            );
        }

        public static IWidget Text(this State<string> data, string key = null) {
            return new Text(
                new TextParam {
                    key = key,
                    data = data
                }
            );
        }

        public static IWidget Margin(
            this IWidget widget,
            [CanBeNull] string key = null,
            EdgeInsets edge = default) {
            return new Margin(
                new MarginParam {
                    key = key,
                    edge = edge,
                    child = widget.ToState(),
                }
            );
        }
    }
}