using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class VerticalSlider : MonoBehaviour
{
    #region Inspector
    [SerializeField]
    private bool startHidden = true;
    [SerializeField]
    private Vector3 offsetDirection;
    [SerializeField]
    private Ease easing;

    [Header("Callbacks")]
    [SerializeField]
    public UnityEvent OnSlideUpDone;
    [SerializeField]
    public UnityEvent OnSlideDownDone;
    #endregion

    private RectTransform rectTransform;
    private Vector3 initialLocalPosition;
    private Vector3 targetPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialLocalPosition = rectTransform.localPosition;
        targetPosition = rectTransform.localPosition + offsetDirection;

        if (startHidden)
        {
            rectTransform.localPosition = targetPosition;
        }
    }

    public Tweener SlideUp(float duration, float delay)
    {
        rectTransform.localPosition = initialLocalPosition;
        return rectTransform.DOLocalMove(targetPosition, duration)
            .From()
            .SetDelay(delay)
            .SetEase(easing)
            .OnComplete(() => OnSlideUpDone?.Invoke());
    }

    public Tweener SlideDown(float duration, float delay)
    {
        rectTransform.localPosition = initialLocalPosition;
        return rectTransform.DOLocalMove(targetPosition, duration)
            .SetDelay(delay)
            .SetEase(easing)
            .OnComplete(() => OnSlideDownDone?.Invoke());
    }
}
