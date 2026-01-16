using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using SA.CrossPlatform.UI;
using Manager;
using UI.Animations;
#if PLATFORM_ANDROID
using UnityEngine.Android;
using SA.Android.App;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
#elif UNITY_IOS
using UnityEngine.iOS;
using SA.iOS.Photos;
#endif

namespace Selfie
{
    public class SelfieSceneController : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup rootCanvasGroup;
        [SerializeField]
        private CanvasGroup swapSelfieModeEffectCanvasGroup;
        [SerializeField]
        private SceneUIFadeIn sceneUiFader;
        [SerializeField]
        private xmgAugmentedFace augmentedFaceController;
        [SerializeField]
        private string mainSceneName;
        [SerializeField]
        private Button exitSceneButton;
        [SerializeField]
        private Button galleryButton;
        [SerializeField]
        private PulseAnimation galleryButtonAnimation;
        [SerializeField]
        private string androidToastMessageOnPhotoShot;

        [Header("Sounds")]
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip exitSceneButtonSound;
        [SerializeField]
        private AudioClip openButtonSound;
        [SerializeField]
        private AudioClip cameraSoundEffect;

        [Header("Animals")]
        [SerializeField]
        private RectTransform hippoIcon;
        [SerializeField]
        private GameObject hippoInteraction;
        [SerializeField]
        private RectTransform snakeIcon;
        [SerializeField]
        private GameObject snakeInteraction;
        [SerializeField]
        private RectTransform lionIcon;
        [SerializeField]
        private GameObject lionInteraction;
        [SerializeField]
        private RectTransform dromedarIcon;
        [SerializeField]
        private GameObject dromedarInteraction;
        [SerializeField]
        private RectTransform spiderIcon;
        [SerializeField]
        private GameObject spiderInteraction;

        private RectTransform currentHighlightedIcon;
        private GameObject currentInteraction;

        private void Start()
        {
            // Reset values
            var defaultIconSCale = new Vector3(0.8f, 0.8f, 0.8f);
            hippoIcon.localScale = defaultIconSCale;
            snakeIcon.localScale = defaultIconSCale;
            lionIcon.localScale = defaultIconSCale;
            dromedarIcon.localScale = defaultIconSCale;
            spiderIcon.localScale = defaultIconSCale;
            hippoInteraction.gameObject.SetActive(false);
            snakeInteraction.gameObject.SetActive(false);
            lionInteraction.gameObject.SetActive(false);
            dromedarInteraction.gameObject.SetActive(false);
            spiderInteraction.gameObject.SetActive(false);
            
#if UNITY_ANDROID
            galleryButton.onClick = null;
            galleryButton.image.color = new Color(1f, 1f, 1f, 0f);
#endif

            //Switch to Spider
            SwitchInteraction(spiderIcon, spiderInteraction);
        }

        public void GoToMainScene()
        {
            StartCoroutine(GoToMainSceneCoroutine());
        }

        public void FlipCamera()
        {
            StartCoroutine(FlipCameraCoroutine());
        }

        private IEnumerator FlipCameraCoroutine()
        {
            swapSelfieModeEffectCanvasGroup.DOFade(1f, 0.05f).WaitForCompletion();
            augmentedFaceController.SwitchCamera();
            yield return new WaitForSeconds(1f);
            swapSelfieModeEffectCanvasGroup.DOFade(0f, 0.15f).WaitForCompletion();
        }

        public void TakeScreenshot()
        {
            StartCoroutine(TakeScreenshotCoroutine());
        }

        public void OpenGallery()
        {
            StartCoroutine(OpenGalleryCoroutine());
        }

        public void StartHippoInteraction()
        {
            SwitchInteraction(hippoIcon, hippoInteraction);
        }

        public void StartSnakeInteraction()
        {
            SwitchInteraction(snakeIcon, snakeInteraction);
        }

        public void StartLionInteraction()
        {
            SwitchInteraction(lionIcon, lionInteraction);
        }

        public void StartDromedaryInteraction()
        {
            SwitchInteraction(dromedarIcon, dromedarInteraction);
        }

        public void StartSpiderInteraction()
        {
            SwitchInteraction(spiderIcon, spiderInteraction);
        }

        private void SwitchInteraction(RectTransform nextIcon, GameObject nextInteraction)
        {
            if (currentInteraction != null)
            {
                if (currentInteraction == nextInteraction)
                {
                    return;
                }

                currentHighlightedIcon.DOScale(0.8f, 0.35f);
                currentHighlightedIcon = null;
                currentInteraction.SetActive(false);
                currentInteraction = null;
            }

            currentHighlightedIcon = nextIcon;
            currentHighlightedIcon.DOScale(1.15f, 0.35f);
            currentInteraction = nextInteraction;
            currentInteraction.SetActive(true);
        }

        public IEnumerator GoToMainSceneCoroutine()
        {
            exitSceneButton.interactable = false;
            audioSource.clip = exitSceneButtonSound;
            audioSource.Play();
            sceneUiFader.autoStart = false;
            sceneUiFader.gameObject.SetActive(true);
            sceneUiFader.delayDuration = 0.1f;
            sceneUiFader.FadeOut();
            yield return new WaitForSeconds(audioSource.clip.length);
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            yield return new WaitForSeconds((audioSource.clip.length * 0.5f) + 0.5f);
            SceneManager.LoadScene(mainSceneName);
        }

        private IEnumerator TakeScreenshotCoroutine()
        {
#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum Speichern von Fotos benötigt diese App Rechte auf deine Fotogallerie.");
                builder.SetPositiveButton("Ok", () =>
                {
                    AN_PermissionsManager.RequestPermission(AMM_ManifestPermission.WRITE_EXTERNAL_STORAGE, result =>
                    {
                        if (result.GrantResults[0].GrantResult == AN_PackageManager.PermissionState.Granted)
                        {
                            StartCoroutine(TakeScreenshotCoroutine());
                        }
                    });
                });
                builder.Build().Show();
            }
            else
            {
                rootCanvasGroup.interactable = false;
                rootCanvasGroup.alpha = 0;
                yield return null;

                GameManager.Instance.TakeScreenshot(() => {
                    galleryButtonAnimation.StartPulseAnimation();
                    PresentAndroidToastMessage(androidToastMessageOnPhotoShot);

                    rootCanvasGroup.alpha = 1;
                    rootCanvasGroup.interactable = true;
                });
            }
#elif UNITY_IOS
            if (ISN_PHPhotoLibrary.AuthorizationStatus != ISN_PHAuthorizationStatus.Authorized)
            {
                ISN_PHPhotoLibrary.RequestAuthorization(status =>
                {
                    if (status == ISN_PHAuthorizationStatus.Authorized)
                    {
                        rootCanvasGroup.interactable = false;
                        rootCanvasGroup.alpha = 0;
                        
                        GameManager.Instance.TakeScreenshot(() => {
                            galleryButtonAnimation.StartPulseAnimation();

                            rootCanvasGroup.alpha = 1;
                            rootCanvasGroup.interactable = true;
                        });
                    }
                    else
                    {
                        var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte",
                            "Zum Speichern von Fotos benötigt diese App Rechte auf deine Fotogallerie.");
                        builder.SetPositiveButton("Einstellungen", () => { Application.OpenURL("App-prefs://"); });
                        builder.SetNegativeButton("Abbrechen", () =>
                        {
                            GameManager.Instance.UserDeclinedCameraUsage = true;
                        });
                        builder.Build().Show();
                    }
                });
            }
            else
            {
                rootCanvasGroup.interactable = false;
                rootCanvasGroup.alpha = 0;
                yield return null;
                GameManager.Instance.TakeScreenshot(() => {
                    galleryButtonAnimation.StartPulseAnimation();

                    rootCanvasGroup.alpha = 1;
                    rootCanvasGroup.interactable = true;
                });
            }
#endif

            yield return null;
        }

        private IEnumerator OpenGalleryCoroutine()
        {
            galleryButton.interactable = false;
            audioSource.clip = openButtonSound;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            galleryButton.interactable = true;
            Application.OpenURL("photos-redirect://");
        }

#if UNITY_ANDROID
        private void PresentAndroidToastMessage(string message)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
            var currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
            AndroidJavaObject javaString=new AndroidJavaObject("java.lang.String", message);
            AndroidJavaObject toast = Toast.CallStatic<AndroidJavaObject> ("makeText", context, javaString, Toast.GetStatic<int>("LENGTH_SHORT"));
            toast.Call ("show");
        }
#endif
    }
}
