using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    [SerializeField]
    private bool autoStart = true;
    [SerializeField]
    private bool fromZero = true;
    [SerializeField]
    private float duration = 1.35f;
    [SerializeField]
    private float delay = 0.75f;
    [SerializeField]
    private Ease easing = Ease.InOutQuad;
    [SerializeField]
    private Vector3 scaleTo;

    private Tweener tweener;
    private Transform instanceTransform;

    private void Awake()
    {
        instanceTransform = transform;
    }

    private void OnEnable()
    {
        if (autoStart)
        {
            StartAnimation();
        }
    }

    public void StartAnimation()
    {
        if (tweener != null && tweener.IsActive())
        {
            tweener.Kill(true);
        }

        if (fromZero)
        {
            instanceTransform.DOScale(scaleTo, duration).SetDelay(delay).SetEase(easing).From();
        }
        else
        {
            instanceTransform.DOScale(scaleTo, duration).SetDelay(delay).SetEase(easing);
        }
    }
}
