using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using Lean.Touch;
using SA.CrossPlatform.UI;
#if PLATFORM_ANDROID
using SA.Android.App;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

public class TreasureRoomController : MonoBehaviour
{
    [HideInInspector]
    public bool clickInSpaceMovesToSelfieScene;
    [SerializeField]
    private string mainScene;
    [SerializeField]
    private string selfieSceneName;
    [SerializeField]
    private LeanTouch leanTouch;

    private void Start()
    {
        if (!GameManager.Instance.HasCollectedTreasure)
        {
            GameManager.Instance.SetTreasureIsCollected();
        }
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerTap += OnFingerTap;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerTap -= OnFingerTap;
    }

    private void OnFingerTap(LeanFinger finger)
    {
        if (!clickInSpaceMovesToSelfieScene || finger.IsOverGui)
        {
            return;
        }

        LoadSelfieScene();
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene(mainScene);
    }

    public void AllowSelfieMode()
    {
        clickInSpaceMovesToSelfieScene = true;
    }

    public void LoadSelfieScene()
    {
        clickInSpaceMovesToSelfieScene = false;

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
                        SceneManager.LoadScene(selfieSceneName);
                    }
                });
            });
            builder.Build().Show();
        }
        else
        {
            SceneManager.LoadScene(selfieSceneName);
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
                SceneManager.LoadScene(selfieSceneName);
            }
#else
            SceneManager.LoadScene(selfieSceneName);
#endif
    }
}
