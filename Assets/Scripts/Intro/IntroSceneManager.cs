using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroSceneManager : MonoBehaviour
{
    [Header("Misc")]
    [SerializeField]
    private string sceneToLoadOnIntroDone;
    [SerializeField]
    private float swipeDeltaThreshold = 24f;

    [Header("Video")]
    [SerializeField]
    private VideoPlayer videoPlayer;

    [Header("Buttons")]
    [SerializeField]
    private Image soundIcon;

    [Header("Indicators")]
    [SerializeField]
    private Sprite activeIndicator;
    [SerializeField]
    private Sprite inactiveIndicator;

    [SerializeField]
    private Image indicator01;
    [SerializeField]
    private Image indicator02;
    [SerializeField]
    private Image indicator03;

    [Header("Sprites")]
    [SerializeField]
    private Sprite toggleOnSprite;
    [SerializeField]
    private Sprite toggleOffSprite;

    private void Start()
    {
        videoPlayer.loopPointReached += OnLoopPointReachedHandler;
        videoPlayer.prepareCompleted += OnVideoPlayerPrepareCompleted;
        PresentAudioState(GameManager.Instance.SoundEffectsOn);
        videoPlayer.Prepare();
    }

    private void OnVideoPlayerPrepareCompleted(VideoPlayer source)
    {
        source.Play();
    }

    private void OnDestroy()
    {
        videoPlayer.loopPointReached -= OnLoopPointReachedHandler;
    }

    private void OnLoopPointReachedHandler(VideoPlayer source)
    {
        GoToMainScene();
    }

    public void ToggleSoundEffects()
    {
        GameManager.Instance.ToggleSoundEffects(!GameManager.Instance.SoundEffectsOn);
        PresentAudioState(GameManager.Instance.SoundEffectsOn);
    }

    public void PresentAudioState(bool state)
    {
        soundIcon.sprite = state ? toggleOnSprite : toggleOffSprite;
    }

    public void HandleOnSwipe(Vector2 direction)
    {
        if (direction.x > swipeDeltaThreshold)
        {
            GoBack();
        }
        else if (direction.x < -swipeDeltaThreshold)
        {
            GoNext();
        }
    }

    private void GoBack()
    {
        if (videoPlayer.time > 108f)
        {
            videoPlayer.time = 31f;
        }
        else if (videoPlayer.time > 31f)
        {
            videoPlayer.time = 0f;
        }
    }

    private void GoNext()
    {
        if (videoPlayer.time < 31f)
        {
            videoPlayer.time = 31f;
        }
        else if (videoPlayer.time < 108f)
        {
            videoPlayer.time = 108f;
        }
    }

    private void LateUpdate()
    {
        if(videoPlayer.time < 31f)
        {
            indicator01.sprite = activeIndicator;
            indicator02.sprite = inactiveIndicator;
            indicator03.sprite = inactiveIndicator;
        }
        else if (videoPlayer.time < 108f)
        {
            indicator01.sprite = inactiveIndicator;
            indicator02.sprite = activeIndicator;
            indicator03.sprite = inactiveIndicator;
        }
        else if (videoPlayer.time >= videoPlayer.length)
        {
            GoToMainScene();
        }
        else
        {
            indicator01.sprite = inactiveIndicator;
            indicator02.sprite = inactiveIndicator;
            indicator03.sprite = activeIndicator;
        }
    }

    public void GoToMainScene()
    {
        if (!GameManager.Instance.PlayerHasSeenIntro)
        {
            GameManager.Instance.SetHasFinishedIntro();
        }

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }

        SceneManager.LoadScene(sceneToLoadOnIntroDone);
    }
}
