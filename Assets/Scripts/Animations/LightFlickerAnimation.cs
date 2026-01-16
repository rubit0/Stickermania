using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

[RequireComponent(typeof(Light))]
public class LightFlickerAnimation : MonoBehaviour
{
    [SerializeField]
    private bool autoStart = true;
    [SerializeField]
    private float duration = 3f;
    [SerializeField]
    private float lightIntensityOffset = 1f;
    [SerializeField]
    private Ease easing = Ease.Flash;

    private Light lightSource;
    private float originalIntensity;
    private TweenerCore<float, float, FloatOptions> tween;

    private void Start()
    {
        lightSource = GetComponent<Light>();
        originalIntensity = lightSource.intensity;
        if (autoStart)
        {
            StartFlickerAnimation();
        }
    }

    public void StartFlickerAnimation()
    {
        lightSource.intensity = GetRandomLightIntensity();
        var targetIntensity = GetRandomLightIntensity();
        tween = lightSource.DOIntensity(targetIntensity + 0.25f, duration).SetEase(easing).SetLoops(-1, LoopType.Yoyo).OnComplete(() => {
            lightSource.intensity = GetRandomLightIntensity();
            tween.endValue = GetRandomLightIntensity();
        });
    }

    private float GetRandomLightIntensity()
    {
        return Random.Range(originalIntensity - lightIntensityOffset, originalIntensity + lightIntensityOffset);
    }
}
