using JetBrains.Annotations;

using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {
    public static class Exts {
        public static EdgeInsets EdgeAll(this int value) {
            return new EdgeInsets {
                left = value,
                right = value,
                top = value,
                bottom = value,
            };
        }

        public static IWidget TextWidget(this string data) {
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