using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class SlideDown : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private Vector3 offsetDirection;
    [SerializeField]
    private Ease easing;
    [SerializeField]
    private float duration = 0.35f;
    [SerializeField]
    private float delay = 3f;
    [SerializeField]
    private bool autoStart = true;
    [SerializeField]
    private UnityEvent OnSlideDownDone;
    #endregion

    private RectTransform rectTransform;
    private Vector3 initialLocalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialLocalPosition = rectTransform.localPosition;
    }

    private void OnEnable()
    {
        if (autoStart)
        {
            StartSlideDown();
        }
    }

    public void StartSlideDown()
    {
        rectTransform.localPosition = initialLocalPosition;
        var target = rectTransform.localPosition + offsetDirection;
        rectTransform.DOLocalMove(target, duration)
            .SetDelay(delay)
            .SetEase(easing)
            .OnComplete(() => OnSlideDownDone?.Invoke());
    }
}
