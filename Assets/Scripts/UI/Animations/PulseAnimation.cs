using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class PulseAnimation : MonoBehaviour
{
    private RectTransform instanceTransform;
    private Tween currentAnimation;

    private void Start()
    {
        instanceTransform = GetComponent<RectTransform>();
    }

    public void StartPulseAnimation()
    {
        if(currentAnimation != null && currentAnimation.active)
        {
            currentAnimation.Complete();
        }

        currentAnimation = instanceTransform
            .DOPunchScale(new Vector3(1.05f, 1.05f, 1.05f), 0.5f, 3)
            .SetDelay(0.2f);
    }
}
