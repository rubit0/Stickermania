using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SelfFader : MonoBehaviour
    {
        [SerializeField]
        private bool autoStartFadeInOut = true;
        [SerializeField]
        private float fadeInDuration = 0.35f;
        [SerializeField]
        private float fadeOutDuration = 0.35f;
        [SerializeField]
        private float delayBettwenFading = 2f;
        [SerializeField]
        private Ease fadeInEasing;
        [SerializeField]
        private Ease fadeOutEasing;
        [SerializeField]
        private UnityEvent onFadeInDone;
        [SerializeField]
        private UnityEvent onFadeOutDone;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (autoStartFadeInOut)
            {
                FadeInAndOut();
            }
        }

        public void FadeIn()
        {
            canvasGroup.DOFade(1f, fadeInDuration)
                .From()
                .SetEase(fadeInEasing)
                .OnComplete(() => onFadeInDone?.Invoke());
        }

        public void FadeInAndOut()
        {
            canvasGroup.DOFade(1f, fadeInDuration)
                .From()
                .SetEase(fadeInEasing)
                .SetDelay(delayBettwenFading)
                .OnComplete(FadeOut);
        }

        public void FadeOut()
        {
            canvasGroup.DOFade(0f, fadeOutDuration)
                .SetEase(fadeOutEasing)
                .OnComplete(() => onFadeOutDone?.Invoke());
        }
    }
}
