using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneUIFadeIn : MonoBehaviour
    {
        public bool autoStart = true;
        public float delayDuration = 1f;
        [SerializeField]
        private float fadeInDuration = 0.5f;
        [SerializeField]
        private Ease fadeInEase = Ease.InSine;
        [SerializeField]
        private UnityEvent onFadeInDone;

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            if (autoStart)
            {
                FadeIn();
            }
        }

        public void FadeIn()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0f, fadeInDuration)
                .SetDelay(delayDuration)
                .SetEase(fadeInEase)
                .OnComplete(() => onFadeInDone?.Invoke());
        }

        public void FadeOut(Action onComplete = null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeInDuration)
                .SetDelay(delayDuration)
                .SetEase(fadeInEase)
                .OnComplete(() => onComplete?.Invoke());
        }
    }
}
