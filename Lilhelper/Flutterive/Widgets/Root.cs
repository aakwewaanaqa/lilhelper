using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;
using UnityEngine.UI;

namespace Lilhelper.Flutterive.Widgets {
    /// <summary>
    /// Root 所需的參數集合（用於建立 Canvas 根節點）
    /// </summary>
    public struct RootParam {
        /// <summary>
        /// Canvas 的設定參數。
        /// </summary>
        public class CanvasBlock {
            /// <summary>用於 ScreenSpaceCamera 模式的相機；為 null 時會改用 ScreenSpaceOverlay</summary>
            public Camera camera;
            /// <summary>相機平面距離 (plane distance)</summary>
            public float planeDistance = 10f;
            /// <summary>Canvas 的排序順序 (sorting order)</summary>
            public int sortingOrder;
        }

        /// <summary>識別用的 key</summary>
        public string key;
        /// <summary>Canvas 參數的可觀察狀態</summary>
        public State<CanvasBlock> canvasParam;
        /// <summary>子 Widget 的可觀察狀態（當變更時會建立新子物件）</summary>
        public State<IWidget> child;
    }

    /// <summary>
    /// 基本 Canvas 根節點。
    /// 建立一個包含 Canvas、CanvasScaler、GraphicRaycaster 與 VerticalLayoutGroup 的根 UI 容器。
    /// </summary>
    public class Root : WidgetBase {
        public readonly RootParam param;
        private GameObject self;
        /// <summary>
        /// 建構子：接收 Root 的參數並保存。
        /// </summary>
        /// <param name="param">Root 的參數</param>
        public Root(in RootParam param) {
            this.param = param;
        }

        /// <summary>
        /// 建立 Root 的 UI 物件並回傳其 RectTransform。
        /// </summary>
        /// <param name="parent">父 RectTransform</param>
        public override RectTransform Build(RectTransform parent = null) {
            new GameObject(nameof(Root))
                .Out(out self)
                .AssignKey(this, param.key)
                .SetRectTransform(out RectTransform rectTransform, parent)
                .SetVerticalLayoutGroup()
                .AddCompAct<Canvas>(it => {
                    // 監聽 Canvas 參數變更，根據是否提供 camera 決定使用 Camera 模式或 Overlay 模式
                    param.canvasParam.ActListenOfHost(val => {
                        if (val.camera.DoExists()) {
                            it.renderMode = RenderMode.ScreenSpaceCamera;
                            it.worldCamera = val.camera;
                            it.planeDistance = val.planeDistance;
                            it.sortingOrder = val.sortingOrder;
                        } else {
                            it.renderMode = RenderMode.ScreenSpaceOverlay;
                        }
                    }, host: self);
                })
                .AddCompAct<CanvasScaler>(it => {
                    it.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    it.referenceResolution = new Vector2(1920, 1080);
                })
                .AddCompAct<GraphicRaycaster>(_ => { })
                ;

            // 當 child 發生變更時：銷毀舊的 child（若存在），並建立新的 child
            param.child?.ActListenOfHost(newValue => {
                param.child.Value?.Kill();
                newValue?.Build(rectTransform);
            }, host: self);

            return rectTransform;
        }
    }

    public static class RootExt {
        /// <summary>
        /// 擴充方法：方便建立 Root Widget。
        /// </summary>
        /// <param name="child">子 Widget</param>
        /// <param name="block">Canvas 參數的可觀察狀態</param>
        public static Root RootWidget(
            this IWidget child,
            Camera cam = null,
            State<RootParam.CanvasBlock> block = null) {
            block ??= new RootParam.CanvasBlock();
            block.Value.camera = cam;
            return new Root(
                new RootParam {
                    canvasParam = block,
                    child = child.ToState(),
                }
            );
        }
    }
}