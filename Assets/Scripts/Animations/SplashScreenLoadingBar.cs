using System.Collections;
using DG.Tweening;
using Manager;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using SA.CrossPlatform.UI;
#endif

namespace Animations
{
    public class SplashScreenLoadingBar : MonoBehaviour
    {
        [SerializeField]
        private bool autoStart;
        [SerializeField]
        private Image loadingBar;

        private void OnEnable()
        {
            if (autoStart)
            {
                StartAnimation();
            }
        }

        public void StartAnimation()
        {
#if UNITY_EDITOR
            StartCoroutine(DevQuickJumpLoad());
#else
        StartCoroutine(FillAnimationCoroutine());
#endif
        }

        private IEnumerator DevQuickJumpLoad()
        {
            yield return null;
            GameManager.Instance.LoadMainScene();
        }

        private IEnumerator FillAnimationCoroutine()
        {
            loadingBar.fillAmount = 0f;
            var target1 = Random.Range(0f, 0.15f);
            var target2 = Random.Range(target1, 0.35f);
            var target3 = Random.Range(target2, 0.55f);
            var target4 = Random.Range(target3, 0.85f);

            yield return loadingBar.DOFillAmount(target1, 0.55f).SetEase(Ease.InCirc).WaitForCompletion();
            yield return loadingBar.DOFillAmount(target2, 0.35f).SetEase(Ease.OutSine).WaitForCompletion();
            yield return loadingBar.DOFillAmount(target3, 1.10f).SetEase(Ease.OutQuad).WaitForCompletion();
            yield return loadingBar.DOFillAmount(target4, 0.75f).SetEase(Ease.InOutCubic).WaitForCompletion();
            yield return loadingBar.DOFillAmount(1f, 0.15f).WaitForCompletion();

#if UNITY_ANDROID
            GameManager.DisplayARWarningMessage(() => GameManager.Instance.LoadMainScene());
#else
            GameManager.Instance.LoadMainScene();
#endif
        }
    }
}
