using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FloatAnimation : MonoBehaviour
{
    [SerializeField]
    private bool autoStart = true;
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 11f, 0);
    [SerializeField]
    private float duration = 1.5f;

    private RectTransform rectTransform;
    private Tween currentAnimation;
    private Vector3 originalPosition;
    private Vector3 targetPosivitePosition;
    private Vector3 targetNegativePosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.localPosition;
        targetPosivitePosition = originalPosition + offset;
        targetNegativePosition = originalPosition - offset;
    }

    private void OnEnable()
    {
        if (autoStart)
        {
            StartAnimation();
        }
    }

    private void OnDisable()
    {
        if(currentAnimation != null && !currentAnimation.IsComplete())
        {
            currentAnimation.Kill(true);
        }

        rectTransform.localPosition = originalPosition;
    }

    public void StartAnimation()
    {
        if (currentAnimation != null && !currentAnimation.IsComplete())
        {
            currentAnimation.Kill(true);
        }

        currentAnimation = rectTransform
            .DOLocalMove(targetPosivitePosition, duration)
            .From(targetNegativePosition)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
