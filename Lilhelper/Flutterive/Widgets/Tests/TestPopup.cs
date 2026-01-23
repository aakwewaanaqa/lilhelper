using UnityEngine.TestTools;
using System.Collections;
using Lilhelper.Flutterive.Widgets;
using UnityEngine;
using UnityEditor;

namespace Lilhelper.Building.Tests {
    public class TestPopup {
        [UnityTest]
        public IEnumerator Build() {
            var framePath = "Assets/Layer Lab/GUI-BlueSky/ResourcesData/Sprites/Components/Popup/Popup00_Frame.png";
            var topFramePath = "Assets/Layer Lab/GUI-BlueSky/ResourcesData/Sprites/Components/Popup/Popup00_TopFrame.png";
            var frameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(framePath);
            var topFrameSprite = AssetDatabase.LoadAssetAtPath<Sprite>(topFramePath);
            new Image(new ImageParam() {
                rendering = new Rendering(
                    data: frameSprite
                ),
                imageType = new SlicedImage(
                    fillCenter: true
                ),
                child = new Image(new ImageParam() {
                    rendering = new Rendering(
                        data: topFrameSprite
                    ),
                    imageType = new SlicedImage(
                        fillCenter: true
                    )
                }).Positioned(anchor: Anchor.Pinned(
                    pinPoint: PinPoint.TopMiddle,
                    lrtb: LRTB.Rect(0, 0, 506, 86),
                    pivot: new Pivot(0.5f, 0f)
                ))
            }).Positioned(
                anchor: Anchor.Pinned(
                    pinPoint: PinPoint.CenterMiddle,
                    lrtb: LRTB.Rect(0, 0, 1180, 800)
            )).Root().Build();

            yield return new WaitForSeconds(5f);
            yield break;
        }
    }
}