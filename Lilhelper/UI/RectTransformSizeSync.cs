using UnityEngine;

namespace Lilhelper.UI {
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("Lilhelper/UI/Rect Transform Size Sync")]
    /// <summary>
    /// 將自身 RectTransform 的 sizeDelta 以比例同步至目標。
    /// Sync this RectTransform's sizeDelta to a target with a ratio.
    /// </summary>
    public class RectTransformSizeSync : MonoBehaviour {
        [SerializeField] private RectTransform self;
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2       ratio;

        private void Reset() {
            self = GetComponent<RectTransform>();
            target = self.parent.GetComponent<RectTransform>();
            ratio = self.sizeDelta / target.sizeDelta;
        }

        private void OnEnable() {
            self   = GetComponent<RectTransform>();
            target = self.parent.GetComponent<RectTransform>();
        }

        private void LateUpdate() {
            self.sizeDelta = target.sizeDelta * ratio;
        }
    }
}
