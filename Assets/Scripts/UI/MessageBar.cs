using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalSlider))]
public class MessageBar : MonoBehaviour
{
    [SerializeField]
    private Text text;
    [SerializeField]
    private float duration = 5f;

    private VerticalSlider slider;
    private Tweener currentAnimation;

    private void Awake()
    {
        slider = GetComponent<VerticalSlider>();
    }

    public void ShowMessage(string message)
    {
        CleanUpAnimation();
        text.text = message;
        currentAnimation = slider.SlideUp(0.35f, 0.1f);
        currentAnimation.OnComplete(() => currentAnimation = slider.SlideDown(duration, 5f));
    }

    public void ShowMessage(string message, float displayDuration)
    {
        CleanUpAnimation();
        text.text = message;
        currentAnimation = slider.SlideUp(0.35f, 0.1f);
        currentAnimation.OnComplete(() => currentAnimation = slider.SlideDown(duration, displayDuration));
    }

    public void CleanUpAnimation()
    {
        if (currentAnimation != null && currentAnimation.IsActive())
        {
            currentAnimation.Kill(true);
        }
    }
}
