using System.Collections;

using NUnit.Framework;

using UnityEngine.TestTools;

using Lilhelper.Flutterive.Widgets;
using Lilhelper.Flutterive.Abstractions;
using UnityEngine;
using UnityEditor;
using Lilhelper.Objs;
using Lilhelper.Reactive;
using Lilhelper.GetIt;

namespace Lilhelper.Tests {
    public class FlutteriveTests {

        public static void FocusInspectorWindow() {
            // 取得 UnityEditor Assembly 中的 InspectorWindow 類型
            var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");

            // 取得該視窗實例 (如果沒有開啟會自動開啟)
            var inspectorWindow = EditorWindow.GetWindow(inspectorType);

            // 聚焦該視窗
            if (inspectorWindow != null) {
                inspectorWindow.Focus();
            }
        }

        private Camera cam;
        private IKeyApi keys;

        [OneTimeSetUp]
        public void OneTimeSetup() {
            FocusInspectorWindow();
            GetItComp.I.Value.Register(() => keys = new KeyApi(), ItLifeTime.Scene);
            new GameObject(nameof(Camera)).AddCompOut(out cam);
        }

        [UnityTest]
        public IEnumerator RootTest() {
            Selection.activeGameObject =
                new Root(new RootParam()).Build().gameObject;
            yield return new WaitForSeconds(5f);
            yield break;
        }

        [UnityTest]
        public IEnumerator SizeTest() {
            var size = new Vector2(100, 50);

            "Hello".TextWidget().Positioned(
                key: "k",
                anchor: Anchor.Pinned(
                    lrtb: LRTB.Rect(
                        size: size)))
            .Root(cam: cam).Build();

            var w = keys.KeyedWidgets["k"];
            var rectTransform = w.Obj.GetComponent<RectTransform>();
            var actualSize = rectTransform.sizeDelta;
            Selection.activeGameObject = rectTransform.gameObject;

            yield return new WaitForSeconds(5f);
            Assert.That(w, Is.TypeOf<Positioned>());
            Assert.That(actualSize, Is.EqualTo(size));

            yield break;
        }
    }
}