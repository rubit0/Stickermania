using UnityEngine;
using DG.Tweening;

public class FloatTransformAnimation : MonoBehaviour
{
    [SerializeField]
    private bool autoStart = true;
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 0.2f, 0);
    [SerializeField]
    private float duration = 1.5f;

    private Transform instanceTransform;
    private Tween currentAnimation;
    private Vector3 originalPosition;
    private Vector3 targetPosivitePosition;
    private Vector3 targetNegativePosition;

    private void Awake()
    {
        instanceTransform = transform;
        originalPosition = instanceTransform.localPosition;
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
        if (currentAnimation != null && !currentAnimation.IsComplete())
        {
            currentAnimation.Kill(true);
        }

        instanceTransform.localPosition = originalPosition;
    }

    public void StartAnimation()
    {
        if (currentAnimation != null && !currentAnimation.IsComplete())
        {
            currentAnimation.Kill(true);
        }

        currentAnimation = instanceTransform
            .DOLocalMove(targetPosivitePosition, duration)
            .From(targetNegativePosition)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
