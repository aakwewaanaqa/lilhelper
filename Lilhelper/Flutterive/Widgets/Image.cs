using Lilhelper.Flutterive.Abstractions;
using Lilhelper.Objs;
using Lilhelper.Reactive;

using UnityEngine;

namespace Lilhelper.Flutterive.Widgets {
    public interface IImageType {

    }

    public class SlicedImage : IImageType {
        public bool fillCenter { get; private set; } = true;
        public SlicedImage(bool fillCenter = true) {
            this.fillCenter = fillCenter;
        }
    }

    public class SimpleImage : IImageType {
        public bool preserveAspect { get; private set; } = false;
        public SimpleImage(bool preserveAspect = false) {
            this.preserveAspect = preserveAspect;
        }
    }

    public class Rendering {
        public Sprite data { get; private set; } = null;
        public Color color { get; private set; } = Color.white;
        public bool raycastTarget { get; private set; } = false;
        public Material material { get; private set; } = null;

        public Rendering(
            Sprite data = null,
            Color? color = null,
            bool raycastTarget = false,
            Material material = null
        ) {
            this.data = data;
            this.color = color ?? Color.white;
            this.raycastTarget = raycastTarget;
            this.material = material;
        }
    }

    public struct ImageParam {
        public string key;
        public State<Rendering> rendering;
        public State<IImageType> imageType;
        public State<IWidget> child;
    }

    public class Image : WidgetBase {

        public ImageParam param;

        public Image(in ImageParam param) {
            this.param = param;
        }

        public override RectTransform Build(RectTransform parent) {
            new GameObject(nameof(Image))
                .Out(out self)
                .AssignKey(this, param.key)
                .SetRectTransform(out RectTransform rectTransform, parent)
                .InheritSize()
                // .SetVerticalLayoutGroup()
                .AddCompAct<UnityEngine.UI.Image>(it => {
                    param.rendering?.ActListenOfHost(newValue => {
                        it.sprite = newValue.data;
                        it.color = newValue.color;
                        it.raycastTarget = newValue.raycastTarget;
                        it.material = newValue.material;
                    }, host: self);

                    param.imageType?.ActListenOfHost(newValue => {
                        switch (newValue) {
                            case SlicedImage slidedImage:
                                it.type = UnityEngine.UI.Image.Type.Sliced;
                                it.fillCenter = slidedImage.fillCenter;
                                break;
                            case SimpleImage simpleImage:
                                it.type = UnityEngine.UI.Image.Type.Simple;
                                it.preserveAspect = simpleImage.preserveAspect;
                                break;
                        }
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

    public static class ImageExt {
        public static Image ImageWidget(
            this Sprite sprite,
            string key = null,
            State<Rendering> rendering = null,
            State<IImageType> imageType = null,
            State<IWidget> child = null
        ) {
            if (rendering == null) {
                rendering = new Rendering(data: sprite)
                    .ToState();
            } else {
                rendering.Value = new Rendering(
                    data: sprite,
                    color: rendering.Value.color,
                    raycastTarget: rendering.Value.raycastTarget,
                    material: rendering.Value.material
                );
            }
            return new Image(new ImageParam {
                key = key,
                rendering = rendering,
                imageType = imageType ?? new SimpleImage(),
                child = child
            });
        }
    }
}
