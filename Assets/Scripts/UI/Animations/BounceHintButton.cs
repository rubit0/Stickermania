using DG.Tweening;
using UnityEngine;

namespace UI.Animations
{
    [RequireComponent(typeof(RectTransform))]
    public class BounceHintButton : MonoBehaviour
    {
        public bool IsShowingHintAnimation { get; private set; }

        [SerializeField]
        private float duration = 0.25f;
        [SerializeField]
        private float scaleAmount = 1.5f;
        [SerializeField]
        private Ease easing = Ease.InBounce;

        private RectTransform rectTransform;
        private Vector3 originalScale;
        private Vector3 targetBounceScale;
        private Tweener activeAnimation;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalScale = rectTransform.localScale;
            targetBounceScale = new Vector3(
                originalScale.x * scaleAmount,
                originalScale.y * scaleAmount,
                originalScale.z * scaleAmount);
        }

        public void StartHintAnimation()
        {
            if (activeAnimation != null
                && activeAnimation.active)
            {
                activeAnimation.Kill(true);
            }

            IsShowingHintAnimation = true;
            rectTransform.localScale = originalScale;
            activeAnimation = rectTransform
                .DOScale(targetBounceScale, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(easing);
        }

        public void StopHintAnimation()
        {
            if (activeAnimation != null
                && activeAnimation.active)
            {
                activeAnimation.Kill(true);
            }

            IsShowingHintAnimation = false;
            rectTransform
                .DOScale(originalScale, duration)
                .SetEase(easing);
        }
    }
}
