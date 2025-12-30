using System.Collections;

using NUnit.Framework;

using UnityEngine.TestTools;

using Lilhelper.Flutterive.Widgets;
using Lilhelper.Flutterive.Abstractions;
using UnityEngine;
using UnityEditor;
using Lilhelper.Objs;
using Lilhelper.Reactive;

namespace Lilhelper.Tests {
    public class FlutteriveTests {
        [UnityTest]
        public IEnumerator SampleTest() {

            new GameObject(nameof(Camera)).AddCompOut(out Camera cam);

            "Hello, Flutterive!".ToState().Out(out State<string> textState);

            new Root(
                new RootParam {
                    canvasParam = new RootParam.CanvasParam {
                        camera = cam,
                        planeDistance = 1f,
                        sortingOrder = 0
                    },
                    child = textState.Text(key: "text").Margin(edge: 20.EdgeAll()).ToState(),
                }
            ).Build(null).gameObject.Out(out GameObject go);

            PrefabUtility.SaveAsPrefabAssetAndConnect(
                go,
                "Assets/Editor/FlutteriveTests_SampleTest.prefab",
                InteractionMode.UserAction
            );

            Assert.Pass();
            yield break;
        }
    }
}