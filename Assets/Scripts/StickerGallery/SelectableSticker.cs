using Domain;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SA.CrossPlatform.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
using SA.Android.App;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

namespace StickerGallery
{
    public class SelectableSticker : MonoBehaviour
    {
        [SerializeField]
        private string fallbackSceneName;
        [SerializeField]
        private string arBridgeSceneName;
        [SerializeField]
        private Image contentImgage;

        private TrackableSticker trackableSticker;

        public void SetupSticker(TrackableSticker trackableSticker)
        {
            this.trackableSticker = trackableSticker;
            contentImgage.sprite = this.trackableSticker.ThumbnailImage;
        }

        public void OpenArScene()
        {
            if (trackableSticker == null)
            {
                return;
            }

            GameManager.Instance.CurrentPlaySticker = trackableSticker;
            if (GameManager.Instance.HasFallbackModeForced)
            {
                SceneManager.LoadScene(fallbackSceneName);
                return;
            }
            
            if (GameManager.Instance.UserDeclinedCameraUsage)
            {
                SceneManager.LoadScene(fallbackSceneName);
                return;
            }

#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum platzieren der Tiere in der erweiterten Realität benötigt diese App Zugriffsrechte auf deine Kamera.");
                builder.SetPositiveButton("Ok", () =>
                {
                    AN_PermissionsManager.RequestPermission(AMM_ManifestPermission.CAMERA, result =>
                    {
                        if (result.GrantResults[0].GrantResult == AN_PackageManager.PermissionState.Granted)
                        {
                            SceneManager.LoadScene(arBridgeSceneName);
                        }
                        else
                        {
                            SceneManager.LoadScene(fallbackSceneName);
                        }
                    });
                });
                builder.SetNegativeButton("Abbrechen", () =>
                {
                    GameManager.Instance.UserDeclinedCameraUsage = true;
                    SceneManager.LoadScene(fallbackSceneName);
                });
                builder.Build().Show();
            }
#elif UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                if (!GameManager.Instance.AskedUserFirstTimeForCameraPermission)
                {
                    var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum platzieren der Tiere in der erweiterten Realität benötigt diese App Zugriffsrechte auf deine Kamera.");
                    builder.SetPositiveButton("Ok", () =>
                    {
                        Application.RequestUserAuthorization(UserAuthorization.WebCam).completed += operation =>
                        {
                            GameManager.Instance.SetAskedFirstTimeForCameraPermission();
                        
                            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                            {
                                SceneManager.LoadScene(arBridgeSceneName);
                            }
                            else
                            {
                                SceneManager.LoadScene(fallbackSceneName);
                            }
                        };
                    });
                    builder.SetNegativeButton("Abbrechen", () =>
                    {
                        GameManager.Instance.UserDeclinedCameraUsage = true;
                        SceneManager.LoadScene(fallbackSceneName);
                    });
                    builder.Build().Show();
                }
                else
                {
                    var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum platzieren der Tiere in der erweiterten Realität benötigt diese App Zugriffsrechte auf deine Kamera.");
                    builder.SetPositiveButton("Einstellungen", () =>
                    {
                        Application.OpenURL("App-prefs://");
                    });
                    builder.SetNegativeButton("Abbrechen", () =>
                    {
                        GameManager.Instance.UserDeclinedCameraUsage = true;
                        SceneManager.LoadScene(fallbackSceneName);
                    });
                    builder.Build().Show();
                }
            }
#endif
            else
            {
                SceneManager.LoadScene(arBridgeSceneName);
            }
        }
    }
}
