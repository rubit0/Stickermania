using System.Collections;
using Manager;
using SA.CrossPlatform.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace AnimalViewer
{
    public class ArSwitchSceneBridge : MonoBehaviour
    {
        private static bool firstRequest = true;

        [SerializeField]
        private string arScene;
        [SerializeField]
        private string fallBackScene;

        private string _deviceModel;
        private readonly string BannedMaker = "huawei";
        private readonly string BannedModel = "mar-lx1";

        private void Awake()
        {
            _deviceModel = SystemInfo.deviceModel.ToLowerInvariant();

            if (GameManager.Instance.HasFallbackModeForced)
            {
                SceneManager.LoadScene(fallBackScene);
                return;
            }

            StartCoroutine(CheckArAvailability());
        }

        private IEnumerator CheckArAvailability()
        {
#if UNITY_ANDROID
            while (ARSession.state == ARSessionState.CheckingAvailability)
            {
                yield return null;
            }

            if (ARSession.state != ARSessionState.Unsupported)
            {
                GameManager.Instance.HasARSupport = false;

                if (!GameManager.Instance.HasDisplayedFirstTimeARUpdatePrompt)
                {
                    GameManager.Instance.HasDisplayedFirstTimeARUpdatePrompt = true;
                    var builder = new UM_NativeDialogBuilder("Hinweis", "Um die lustigen Tiere in AR zu erleben, führe auf deinem Handy ein Update auf die Version Android 8.0 aus.");
                    builder.SetPositiveButton("Ok", () =>
                    {
                        SceneManager.LoadScene(fallBackScene);
                    });
                    builder.Build().Show();
                }
                else
                {
                    SceneManager.LoadScene(fallBackScene);
                }
                yield break;
            }

            var canAccessCamera = Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
            var canAccessCamera = Application.HasUserAuthorization(UserAuthorization.WebCam);
#endif

            if (!canAccessCamera && firstRequest)
            {
                GameManager.Instance.HasARSupport = false;
                SceneManager.LoadScene(fallBackScene);
            }
            else
            {
                firstRequest = false;

                while (ARSession.state == ARSessionState.None ||
                        ARSession.state == ARSessionState.Installing ||
                        ARSession.state == ARSessionState.CheckingAvailability)
                {
                    yield return null;
                }

                if (ARSession.state == ARSessionState.Unsupported)
                {
                    GameManager.Instance.HasARSupport = false;
                }
                else
                {
                    yield return ARSession.Install();
                    while (ARSession.state == ARSessionState.Installing)
                    {
                        yield return null;
                    }

                    GameManager.Instance.HasARSupport = ARSession.state != ARSessionState.NeedsInstall;
                }

#if UNITY_ANDROID
                if (GameManager.Instance.HasARSupport)
                {
                    if (GameManager.SuppressArHintMessage)
                    {
                        GameManager.SuppressArHintMessage = false;
                        SceneManager.LoadScene(arScene);
                    }
                    else
                    {
                        GameManager.DisplayARWarningMessage(() => SceneManager.LoadScene(arScene));
                    }
                }
                else
                {
                    SceneManager.LoadScene(fallBackScene);
                }
#else
                var sceneToLoad = GameManager.Instance.HasARSupport ? arScene : fallBackScene;
                SceneManager.LoadScene(sceneToLoad);
#endif
            }
        }

        private bool IsExcludedDevice(string deviceModel)
        {
            return deviceModel.Contains(BannedMaker) && deviceModel.Contains(BannedModel);
        }
    }
}
