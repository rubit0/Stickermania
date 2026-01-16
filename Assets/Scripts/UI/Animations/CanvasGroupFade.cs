using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Animations
{

    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFade : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private float fadeInDuration = 0.5f;
        [SerializeField]
        private float fadeOutDuration = 0.25f;
        [SerializeField]
        private Ease fadeInEase = Ease.InSine;
        [SerializeField]
        private Ease fadeOutEase = Ease.OutSine;
        [SerializeField]
        private bool delayFadeIn = false;
        [SerializeField]
        private bool delayFadeOut = false;
        [SerializeField]
        private float delayDuration = 1f;
        [SerializeField]
        private UnityEvent onFadeInDone;
        [SerializeField]
        private UnityEvent onFadeOutDone;
        #endregion

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void HideImmediately()
        {
            canvasGroup.alpha = 0f;
        }

        public void ShowImmediately()
        {
            canvasGroup.alpha = 1f;
        }

        public void FadeIn()
        {
            HideImmediately();
            if (delayFadeIn)
            {
                canvasGroup.DOFade(1f, fadeInDuration)
                    .SetEase(fadeInEase)
                    .SetDelay(delayDuration)
                    .OnComplete(() => onFadeInDone?.Invoke());
            }
            else
            {
                canvasGroup.DOFade(1f, fadeInDuration)
                    .SetEase(fadeInEase)
                    .OnComplete(() => onFadeInDone?.Invoke());
            }
        }

        public void FadeOut()
        {
            if (delayFadeOut)
            {
                ShowImmediately();
                canvasGroup.DOFade(0f, fadeInDuration)
                    .SetEase(fadeOutEase)
                    .SetDelay(delayDuration)
                    .OnComplete(() => onFadeOutDone?.Invoke());
            }
            else
            {
                ShowImmediately();
                canvasGroup.DOFade(0f, fadeInDuration)
                    .SetEase(fadeOutEase)
                    .OnComplete(() => onFadeOutDone?.Invoke());
            }
        }
    }
}
