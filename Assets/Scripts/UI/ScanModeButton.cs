using UnityEngine;
using UnityEngine.SceneManagement;
using Manager;
using SA.CrossPlatform.UI;
#if UNITY_ANDROID
using SA.Android.App;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
#endif
#if PLATFORM_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

public class ScanModeButton : MonoBehaviour
{
    [SerializeField]
    private string stickerSceneName = "StickerScan";

    public void StartScanMode()
    {
        var remaining = GameManager.Instance.TrackableStickers.Count - GameManager.Instance.CollectedStickers.Count;
        if(remaining == 0)
        {
            return;
        }

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum Scannen der Sticker benötigt diese App Zugriffsrechte auf deine Kamera.");
            builder.SetPositiveButton("Ok", () =>
            {
                AN_PermissionsManager.RequestPermission(AMM_ManifestPermission.CAMERA, result =>
                {
                    if (result.GrantResults[0].GrantResult == AN_PackageManager.PermissionState.Granted)
                    {
                        GameManager.DisplayARWarningMessage(() => SceneManager.LoadScene(stickerSceneName));
                    }
                });
            });
            builder.Build().Show();
        }
        else
        {
            GameManager.DisplayARWarningMessage(() => SceneManager.LoadScene(stickerSceneName));
        }
#elif UNITY_IOS
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            if (!GameManager.Instance.AskedUserFirstTimeForCameraPermission)
            {
                var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum Scannen der Sticker benötigt diese App Zugriffsrechte auf deine Kamera.");
                builder.SetPositiveButton("Ok", () =>
                {
                    Application.RequestUserAuthorization(UserAuthorization.WebCam).completed += operation =>
                    {
                        GameManager.Instance.SetAskedFirstTimeForCameraPermission();
                        
                        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
                        {
                            SceneManager.LoadSceneAsync(stickerSceneName);
                        }
                    };
                });
                builder.SetNegativeButton("Abbrechen", () =>
                {
                    GameManager.Instance.UserDeclinedCameraUsage = true;
                });
                builder.Build().Show();
            }
            else
            {
                var builder = new UM_NativeDialogBuilder("Fehlende Zugriffsrechte", "Zum Scannen der Sticker benötigt diese App Zugriffsrechte auf deine Kamera.");
                builder.SetPositiveButton("Einstellungen", () =>
                {
                    Application.OpenURL("App-prefs://");
                });
                builder.SetNegativeButton("Abbrechen", () =>
                {
                    GameManager.Instance.UserDeclinedCameraUsage = true;
                });
                builder.Build().Show();
            }
        }
        else
        {
            SceneManager.LoadSceneAsync(stickerSceneName);
        }
#else
        SceneManager.LoadSceneAsync(stickerSceneName);
#endif
    }
}
