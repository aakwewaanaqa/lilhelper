using System.Collections.Generic;

using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;

namespace Lilhelper.Flutterive.Widgets {
    public enum PinPoint {
        TopLeft,
        TopMiddle,
        TopRight,
        CenterLeft,
        CenterMiddle,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public static class PinPointExt {
        public static Vector2 ToVector2(this PinPoint pinPoint) {
            return pinPoint switch {
                PinPoint.TopLeft => new Vector2(0, 1),
                PinPoint.TopMiddle => new Vector2(0.5f, 1),
                PinPoint.TopRight => new Vector2(1, 1),
                PinPoint.CenterLeft => new Vector2(0, 0.5f),
                PinPoint.CenterMiddle => new Vector2(0.5f, 0.5f),
                PinPoint.CenterRight => new Vector2(1, 0.5f),
                PinPoint.BottomLeft => new Vector2(0, 0),
                PinPoint.BottomCenter => new Vector2(0.5f, 0),
                PinPoint.BottomRight => new Vector2(1, 0),
                _ => new Vector2(0.5f, 0.5f),
            };
        }
    }

    public class MinMax {
        public float xMin { get; private set; }
        public float xMax { get; private set; }
        public float yMin { get; private set; }
        public float yMax { get; private set; }

        public bool IsXExpandable => xMin != xMax;
        public bool IsYExpandable => yMin != yMax;

        public MinMax(float xMin = 0.5f, float xMax = 0.5f, float yMin = 0.5f, float yMax = 0.5f) {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;
        }
        public static MinMax Row(float y = 0.5f, float xMin = 0, float xMax = 1) {
            return new MinMax {
                xMin = xMin,
                xMax = xMax,
                yMin = y,
                yMax = y,
            };
        }
        public static MinMax Column(float x = 0.5f, float yMin = 0, float yMax = 1) {
            return new MinMax {
                xMin = x,
                xMax = x,
                yMin = yMin,
                yMax = yMax,
            };
        }
        public static MinMax Expand() {
            return new MinMax {
                xMin = 0,
                xMax = 1,
                yMin = 0,
                yMax = 1,
            };
        }
        public static MinMax Pinned(Vector2 anchor) {
            return new MinMax {
                xMin = anchor.x,
                xMax = anchor.x,
                yMin = anchor.y,
                yMax = anchor.y,
            };
        }
        public static MinMax Pinned(PinPoint pinPoint = PinPoint.CenterMiddle) {
            Vector2 anchor = pinPoint.ToVector2();
            return new MinMax {
                xMin = anchor.x,
                xMax = anchor.x,
                yMin = anchor.y,
                yMax = anchor.y,
            };
        }
    }

    public class LRTB {
        public float left { get; private set; }
        public float right { get; private set; }
        public float top { get; private set; }
        public float bottom { get; private set; }
        public Vector2 AnchoredPosition => new Vector2(left, top);
        public Vector2 Size => new Vector2(right, bottom);

        public LRTB(float l = 0, float r = 0, float t = 100, float b = 100) {
            left = l;
            right = r;
            top = t;
            bottom = b;
        }
        public static LRTB Row(float left = 0, float right = 0, float y = 0, float height = 0) {
            return new LRTB {
                left = left,
                right = right,
                top = y,
                bottom = height,
            };
        }
        public static LRTB Column(float top = 0, float bottom = 0, float x = 0, float width = 0) {
            return new LRTB {
                left = x,
                right = width,
                top = top,
                bottom = bottom,
            };
        }
        public static LRTB Rect(Vector2 pos = default, Vector2 size = default) {
            return new LRTB {
                left = pos.x,
                right = size.x,
                top = pos.y,
                bottom = size.y,
            };
        }
        public static LRTB Rect(float x, float y, float width, float height) {
            return new LRTB {
                left = x,
                right = width,
                top = y,
                bottom = height,
            };
        }
        public static LRTB Rect(Rect rect) {
            return new LRTB {
                left = rect.x,
                right = rect.width,
                top = rect.y,
                bottom = rect.height,
            };
        }
    }

    public class Pivot {
        public float x { get; private set; } = 0.5f;
        public float y { get; private set; } = 0.5f;

        public Pivot(float x = 0.5f, float y = 0.5f) {
            this.x = x;
            this.y = y;
        }

        public static Pivot Pinned(PinPoint pinPoint = PinPoint.CenterMiddle) {
            Vector2 anchor = pinPoint.ToVector2();
            return new Pivot {
                x = anchor.x,
                y = anchor.y,
            };
        }
    }

    public class Anchor {
        public MinMax anchor { get; private set; }
        public LRTB lrtb { get; private set; }
        public Pivot pivot { get; private set; }

        public Vector2 Min => new(anchor.xMin, anchor.yMin);
        public Vector2 Max => new(anchor.xMax, anchor.yMax);

        public Anchor(MinMax anchor = null, LRTB lrtb = null, Pivot pivot = null) {
            this.anchor = anchor ?? new MinMax();
            this.lrtb = lrtb ?? new LRTB();
            this.pivot = pivot ?? new Pivot();
        }

        /// <summary>
        /// 如果是定位定大小的錨點，用 <see cref="LRTB.Rect(Vector2, Vector2)"/> 設定位置與大小
        /// </summary>
        public static Anchor Pinned(PinPoint pinPoint = PinPoint.CenterMiddle, Pivot pivot = null, LRTB lrtb = null) {
            return new Anchor {
                anchor = MinMax.Pinned(pinPoint),
                lrtb = lrtb ?? new LRTB(),
                pivot = pivot ?? Pivot.Pinned(pinPoint),
            };
        }

        public static Anchor Expand(LRTB lrtb = null) {
            return new Anchor {
                anchor = MinMax.Expand(),
                lrtb = lrtb ?? new LRTB(0, 0, 0, 0),
                pivot = new Pivot(),
            };
        }
    }

    public static class AnchorExt {
        public static RectTransform SetAnchor(this RectTransform self, Anchor anchor) {
            self.anchoredPosition = anchor.lrtb.AnchoredPosition;
            self.sizeDelta = anchor.lrtb.Size;
            self.pivot = new Vector2(anchor.pivot.x, anchor.pivot.y);
            self.anchorMin = anchor.Min;
            self.anchorMax = anchor.Max;
            return self;
        }
    }

    public struct PositionedParam {
        public string key;
        public State<Anchor> anchor;
        public State<IWidget> child;
    }

    public class Positioned : WidgetBase {
        private PositionedParam param;

        public Positioned(PositionedParam param) {
            this.param = param;
        }

        public override RectTransform Build(RectTransform parent) {
            new GameObject(nameof(Positioned))
                .Out(out self)
                .AssignKey(this, param.key)
                .SetRectTransform(out RectTransform rectTransform, parent)
                // .SetVerticalLayoutGroup()
                ;

            param.anchor?.ActListenOfHost(newValue => {
                rectTransform.SetAnchor(newValue);
            }, host: self);

            param.child?.ActListenOfHost(newValue => {
                param.child.Value?.Kill();
                newValue?.Build(rectTransform);
            }, host: self);

            return rectTransform;
        }
    }

    public static class PositionedExt {
        public static Positioned Positioned(
            this IWidget child,
            string key = null,
            State<Anchor> anchor = null) {
            return new Positioned(
                new PositionedParam {
                    key = key,
                    anchor = anchor,
                    child = child.ToState(),
                }
            );
        }
    }
}
