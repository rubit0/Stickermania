using System.Collections;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SA.CrossPlatform.UI;
#if PLATFORM_ANDROID
using SA.Android.App;
using SA.Android.Content;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

namespace StickerGallery
{
    public class StickerGallerySceneController : MonoBehaviour
    {
        #region Inspector
        [Header("Unlockables")]
        [SerializeField]
        private Button mazeModeButton;
        [SerializeField]
        private Sprite unlockedMazeImage;
        [SerializeField]
        private GameObject mazeLockedPanel;

        [Space]
        [SerializeField]
        private Button selfieModeButton;
        [SerializeField]
        private Sprite unlockedSelfieImage;
        [SerializeField]
        private GameObject selfieLockedPanel;

        [Space]
        [SerializeField]
        private GameObject lockImage;

        [Header("Misc")]
        [SerializeField]
        private float loadingSceneDelay = 2f;

        [SerializeField]
        private string mazeSceneName;
        [SerializeField]
        private GameObject mazeSceneLoadingPanel;

        [SerializeField]
        private string selfieSceneName;
        #endregion

#if UNITY_ANDROID
        private static bool didCheckForAppStart;
#endif

        private void Start()
        {
            // Check unlockables
            if (GameManager.Instance.CollectedStickers.Count > 11)
            {
                mazeModeButton.interactable = true;
                mazeModeButton.image.sprite = unlockedMazeImage;
                lockImage.gameObject.SetActive(false);
            }

#if UNITY_ANDROID
            if (!GameManager.Instance.AppRatingFlowDisabled)
            {
                if (GameManager.Instance.DisplayedAnimalVisualizationsAmount == 2 
                    && !GameManager.Instance.Displayed2ndRatingDialogue)
                {
                    GameManager.Instance.Displayed2ndRatingDialogue = true;
                    InitRateUsDialogueFlow();
                }
                else if (GameManager.Instance.DisplayedAnimalVisualizationsAmount == 8 
                    && !GameManager.Instance.Displayed8thRatingDialogue)
                {
                    GameManager.Instance.Displayed8thRatingDialogue = true;
                    InitRateUsDialogueFlow();
                }
                else if (GameManager.Instance.DisplayedAnimalVisualizationsAmount == 16
                         && !GameManager.Instance.Displayed16thRatingDialogue)
                {
                    GameManager.Instance.Displayed16thRatingDialogue = true;
                    InitRateUsDialogueFlow();
                }
                else if (GameManager.Instance.HasCollectedTreasure
                         && !GameManager.Instance.DisplayedSelfieRatingDialogue)
                {
                    GameManager.Instance.DisplayedSelfieRatingDialogue = true;
                    InitRateUsDialogueFlow();
                }
                else if(GameManager.Instance.DisplayedSelfieRatingDialogue)
                {
                    if (!didCheckForAppStart)
                    {
                        didCheckForAppStart = true;
                        GameManager.Instance.AppStartAmount++;
                    }

                    if (GameManager.Instance.AppStartAmount >= 5)
                    {
                        GameManager.Instance.AppStartAmount = 0;
                        InitRateUsDialogueFlow();
                    }
                }
            }
#endif

            if (GameManager.Instance.HasCollectedTreasure)
            {
                selfieModeButton.interactable = true;
                selfieModeButton.image.sprite = unlockedSelfieImage;
            }
        }

        public void LoadMazeScene()
        {
            // Check unlockables
            if (GameManager.Instance.CollectedStickers.Count < 12)
            {
                mazeLockedPanel.SetActive(true);
                return;
            }

            if (!mazeModeButton.interactable)
            {
                return;
            }

            mazeModeButton.interactable = false;
            StartCoroutine(LoadMazeSceneCoroutine(mazeSceneName));
        }

        public void LoadSelfieScene()
        {
            if (!GameManager.Instance.HasCollectedTreasure)
            {
                selfieLockedPanel.SetActive(true);
                return;
            }

            if (!selfieModeButton.interactable)
            {
                return;
            }

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum aufnehmen von Selfies benötigt diese App Rechte auf deine Kamera.");
                builder.SetPositiveButton("Ok", () =>
                {
                    AN_PermissionsManager.RequestPermission(AMM_ManifestPermission.CAMERA, result =>
                    {
                        if (result.GrantResults[0].GrantResult == AN_PackageManager.PermissionState.Granted)
                        {
                            selfieModeButton.interactable = false;
                            GameManager.DisplayARWarningMessage(() => SceneManager.LoadScene(selfieSceneName));
                        }
                    });
                });
                builder.Build().Show();
            }
            else
            {
                selfieModeButton.interactable = false;
                GameManager.DisplayARWarningMessage(() => SceneManager.LoadScene(selfieSceneName));
            }
#elif UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                if (!GameManager.Instance.AskedUserFirstTimeForCameraPermission)
                {
                    var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum aufnehmen von Selfies benötigt diese App Rechte auf deine Kamera.");
                    builder.SetPositiveButton("Ok", () =>
                    {
                        Application.RequestUserAuthorization(UserAuthorization.WebCam).completed += operation =>
                        {
                            GameManager.Instance.SetAskedFirstTimeForCameraPermission();

                            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                            {
                                SceneManager.LoadScene(selfieSceneName);
                            }
                        };
                    });
                    builder.Build().Show();
                }
                else
                {
                    var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum aufnehmen von Selfies benötigt diese App Rechte auf deine Kamera.");
                    builder.SetPositiveButton("Einstellungen", () => { Application.OpenURL("App-prefs://"); });
                    builder.SetNegativeButton("Abbrechen", () =>
                    {
                        GameManager.Instance.UserDeclinedCameraUsage = true;
                    });
                    builder.Build().Show();
                }
            }
            else
            {
                selfieModeButton.interactable = false;
                SceneManager.LoadScene(selfieSceneName);
            }
#else
            selfieModeButton.interactable = false;
            SceneManager.LoadScene(selfieSceneName);
#endif
        }

        private IEnumerator LoadMazeSceneCoroutine(string sceneName)
        {
            mazeSceneLoadingPanel.gameObject.SetActive(true);
            yield return new WaitForSeconds(loadingSceneDelay);
            SceneManager.LoadScene(sceneName);
        }

#if UNITY_ANDROID
        private void InitRateUsDialogueFlow()
        {
            var builder = new UM_NativeDialogBuilder("Pssst... ich bins, Oskar!", "Entschuldige, dass ich dich beim Spielen störe. Ich wollte nur kurz nachfragen, wie dir die neue Stickermania-App gefällt?");
            builder.SetPositiveButton("Gefällt mir!", DisplayPreparingRateUsDialogue);
            builder.SetNegativeButton("Gefällt mir nicht!", DisplayDeclinedRatingDialogue);
            builder.Build().Show();
        }

        private void DisplayPreparingRateUsDialogue()
        {
            var builder = new UM_NativeDialogBuilder("Schön, dass sie dir gefällt!", "Ich würde mich sehr freuen, wenn du dir einen Moment Zeit nimmst, um die App zu bewerten. Es dauert auch nicht lange, versprochen! Bitte dabei deine Eltern um ihre Hilfe.");
            builder.SetPositiveButton("Bewertung abgeben", () =>
            {
                GameManager.Instance.AppRatingFlowDisabled = true;
                DisplayRateUsDialogue();
            });
            builder.SetNeutralButton("Vielleicht später", () =>
            {

            });
            builder.SetNegativeButton("Nein, danke", () => { GameManager.Instance.AppRatingFlowDisabled = true; });
            builder.Build().Show();
        }

        private void DisplayDeclinedRatingDialogue()
        {
            var builder = new UM_NativeDialogBuilder("Oje, das tut mir leid!", "Ich würde mich sehr freuen, wenn du mir genauer erklären könntest, was dir nicht gefällt. Nur so können wir die App für dich verbessern. Schreibe eine E-Mail an stickermania@spar.at");
            builder.SetPositiveButton("Feedback geben", EmailUs);
            builder.SetNegativeButton("Nein, danke", () => { });
            builder.Build().Show();
        }

        private void DisplayRateUsDialogue()
        {
            var appName = Application.productName;
            var uri = new System.Uri("market://details?id=at.spar.stickermania");
            var viewIntent = new AN_Intent(AN_Intent.ACTION_VIEW, uri);
            AN_MainActivity.Instance.StartActivity(viewIntent);
        }

        private void EmailUs()
        {
            const string email = "stickermania@spar.at";
            var subject = EscapeURL("Feedback Stickermania App");
            Application.OpenURL("mailto:" + email + "?subject=" + subject);
        }

        private string EscapeURL(string url)
        {
            return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        }
#endif
    }
}
