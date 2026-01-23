using Lilhelper.Flutterive.Abstractions;
using Lilhelper.GetIt;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {
    public abstract class WidgetBase : IWidget {
        protected GameObject self;

        public GameObject Obj => self;

        public abstract RectTransform Build(RectTransform parent);

        public virtual bool Kill() {
            if (self.DoExists()) {
                Object.Destroy(self);
                return true;
            }
            return false;
        }
    }

    public static class WidgetBaseExts {
        internal static GameObject AssignKey(
            this GameObject go,
            IWidget widget,
            string key) {
            if (string.IsNullOrEmpty(key)) return go;
            var keys = GetItComp.I.Value.Get<IKeyApi>();
            if (keys.IsNull()) return go;
            keys.KeyedWidgets[key] = widget;
            return go;
        }

        internal static GameObject SetRectTransform(
            this GameObject self,
            out RectTransform rectTransform,
            Transform parent = null) {
            return self
                .EnsureCompActOut(it => it.SetParent(parent, false), out rectTransform);
        }

        internal static GameObject SetRectTransform(
            this GameObject self,
            Transform parent = null) {
            return self
                .EnsureCompAct<RectTransform>(it => it.SetParent(parent, false));
        }

        internal static GameObject SetVerticalLayoutGroup(
            this GameObject self,
            out VerticalLayoutGroup group,
            RectOffset padding = null) {
            return self
                .EnsureCompActOut(it => {
                    it.padding = padding;
                    it.childAlignment = TextAnchor.UpperLeft;
                    it.childControlHeight = false;
                    it.childControlWidth = false;
                    it.childForceExpandHeight = false;
                    it.childForceExpandWidth = false;
                    it.spacing = 0;
                }, out group);
        }

        internal static GameObject SetVerticalLayoutGroup(
            this GameObject self,
            RectOffset padding = null) {
            return self
                .EnsureCompAct<VerticalLayoutGroup>(it => {
                    it.padding = padding ?? new RectOffset();
                    it.childAlignment = TextAnchor.UpperLeft;
                    it.childControlHeight = false;
                    it.childControlWidth = false;
                    it.childForceExpandHeight = false;
                    it.childForceExpandWidth = false;
                    it.spacing = 0;
                });
        }

        internal static GameObject SetContentSizeFitter(
            this GameObject self,
            out ContentSizeFitter fitter) {
            return self
                .EnsureCompActOut(it => {
                    it.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    it.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }, out fitter);
        }

        internal static GameObject SetContentSizeFitter(
            this GameObject self) {
            return self
                .EnsureCompAct<ContentSizeFitter>(it => {
                    it.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                    it.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                });
        }

        internal static GameObject InheritSize(this GameObject self) {
            self.GetCompAct<RectTransform>(it => it.SetAnchor(anchor: Anchor.Expand()));
            return self;
        }

        internal static GameObject SetSizing(
            this GameObject self,
            State<ISizing> sizing = null) {
            sizing ??= new Inherit();
            sizing.ActListenOfHost(newValue => {
                switch (newValue) {
                    default:
                    case FitContent:
                        self.SetContentSizeFitter();
                        break;
                    case Inherit:
                        self.GetCompAct<RectTransform>(it => it.SetAnchor(anchor: Anchor.Expand()));
                        break;
                    case Size size:
                        self.GetCompAct<RectTransform>(it => it.sizeDelta = size.size);
                        break;
                }
            }, host: self);
            return self;
        }
    }
}