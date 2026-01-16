using System.Collections;
using System.Collections.Generic;
using Domain;
using Manager;
using UI.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using SA.CrossPlatform.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
using SA.Android.App;
using SA.Android.Content.Pm;
using SA.Android.Manifest;
#elif UNITY_IOS
using UnityEngine.iOS;
using SA.iOS.Photos;
#endif

namespace XR
{
    public class ARSceneManager : MonoBehaviour
    {
        [Header("Misc")] [SerializeField] private string mainSceneName;
        [SerializeField] private Camera camera;
        [SerializeField] private ARVisualizationsController arVisualizationsController;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private AudioSource musicSoundSource;
        [SerializeField] private AudioClip cameraSoundEffect;

        [SerializeField] [Tooltip("Time in seconds")]
        private float durationUntilHint;

        [SerializeField] private Material tapToPlaceMaterial;
        [SerializeField] private Material shadowPlaneMaterial;
        [SerializeField] private float modelScaleFactor = 0.25f;

        [Header("UI Elements")] [SerializeField]
        private CanvasGroup rootCanvasGroup;

        [SerializeField] private MessageBar messageBar;
        [SerializeField] private BounceHintButton hintButton;
        [SerializeField] private GameObject tapToPlaceImage;
        [SerializeField] private GameObject swipeToRotateImage;
        [SerializeField] private CanvasGroupFade buttonsCanvas;
        [SerializeField] private CanvasGroupFade hintCanvas;
        [SerializeField] private CanvasGroupFade howToCanvas;
        [SerializeField] private PulseAnimation cameraIconPulse;

        [Header("Messages")] [SerializeField] private string movePhoneMessage;
        [SerializeField] private string tapToPlaceMessage;
        [SerializeField] private string rotateModelMessage;

        private InteractiveModel placedModelInstance;
        private bool checkShowingHint;

        private void Start()
        {
            rootCanvasGroup.interactable = true;
            rootCanvasGroup.alpha = 1;
            buttonsCanvas.HideImmediately();
            StartCoroutine(CheckIfShouldShowHintCoroutine());
            StartCoroutine(CheckForInitialPlaneCoroutine(true));
            GameManager.Instance.DisplayedAnimalVisualizationsAmount++;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        public void GoToMainScene()
        {
            SceneManager.LoadScene(mainSceneName);
        }

        public void StopCheckingShowHint()
        {
            checkShowingHint = false;
            StopCoroutine(CheckIfShouldShowHintCoroutine());
        }

        public void StartPositionModelMode()
        {
            buttonsCanvas.FadeOut();
            placedModelInstance.enabled = false;
            StartCoroutine(CheckForInitialPlaneCoroutine(false));
        }

        public void TakeScreenshot()
        {
            StartCoroutine(TakeScreenshotCoroutine());
        }

        public void ShowHelpScreen()
        {
            if (placedModelInstance == null)
            {
                hintCanvas.gameObject.SetActive(true);
                hintCanvas.FadeIn();
            }
            else
            {
                howToCanvas.gameObject.SetActive(true);
                howToCanvas.FadeIn();
            }
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
                    cameraIconPulse.StartPulseAnimation();
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

                        GameManager.Instance.TakeScreenshot(() =>
                        {
                            cameraIconPulse.StartPulseAnimation();
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

                GameManager.Instance.TakeScreenshot(() =>
                {
                    cameraIconPulse.StartPulseAnimation();
                    rootCanvasGroup.alpha = 1;
                    rootCanvasGroup.interactable = true;
                });
            }
#endif

            yield return null;
        }

        private IEnumerator CheckIfShouldShowHintCoroutine()
        {
            // setup coroutine
            checkShowingHint = true;
            const float waitTime = 0.25f;
            var waiter = new WaitForSeconds(waitTime);
            var waitedTime = 0f;

            while (checkShowingHint || arVisualizationsController.PlaneManager.trackables.count < 1)
            {
                yield return waiter;
                waitedTime += waitTime;

                // Show hint if waittime is reached
                if (waitedTime >= durationUntilHint)
                {
                    hintButton.StartHintAnimation();
                    checkShowingHint = false;
                    break;
                }
            }
        }

        private IEnumerator CheckForInitialPlaneCoroutine(bool withDelay)
        {
            // Enable AR trackables visualization
            arVisualizationsController.ChangePointCloudVisualization(ARVisualizationsController.VisualizationMode.Show);
            if (withDelay)
            {
                messageBar.ShowMessage(movePhoneMessage, 25f);
                arVisualizationsController.ChangePlaneVisualization(ARVisualizationsController.VisualizationMode
                    .Invisible);
            }
            else
            {
                arVisualizationsController.ChangePlaneVisualization(ARVisualizationsController.VisualizationMode.Show);
            }

            // Wait until a plane is detected
            while (arVisualizationsController.PlaneManager.trackables.count < 1)
            {
                yield return null;
            }

            if (withDelay)
            {
                yield return new WaitForSeconds(3f);
                arVisualizationsController.ChangePlaneVisualization(ARVisualizationsController.VisualizationMode.Show);
            }

            // UI
            hintButton.StopHintAnimation();
            checkShowingHint = false;
            StopCoroutine(CheckIfShouldShowHintCoroutine());

            // Go to next stage
            StartCoroutine(CheckForPlanePlacement());
        }

        private IEnumerator CheckForPlanePlacement()
        {
            messageBar.ShowMessage(tapToPlaceMessage, 25f);
            tapToPlaceImage.gameObject.SetActive(true);

            var doCheckForPlacement = true;
            while (doCheckForPlacement)
            {
                yield return null;

                // Check if screen has been touched but the hint canvas is not active
                if (Input.touchCount > 0 &&
                    (!hintCanvas.gameObject.activeInHierarchy
                     || !howToCanvas.gameObject.activeInHierarchy
                     || !EventSystem.current.IsPointerOverGameObject()))
                {
                    // ... and it is the start of a touch
                    var touchInfo = Input.GetTouch(0);
                    if (touchInfo.phase != TouchPhase.Began)
                    {
                        continue;
                    }

                    // Try raycast on a place to place an animal
                    var hitResults = new List<ARRaycastHit>();
                    if (raycastManager.Raycast(touchInfo.position, hitResults, TrackableType.PlaneWithinPolygon))
                    {
                        // Disable AR trackables visualization
                        arVisualizationsController.ChangePointCloudVisualization(ARVisualizationsController
                            .VisualizationMode.Invisible);
                        arVisualizationsController.ChangePlaneVisualization(ARVisualizationsController.VisualizationMode
                            .Shadow);

                        // Handle placing and UI
                        var hitPose = hitResults[0].pose;

                        if (placedModelInstance == null)
                        {
                            placedModelInstance = Instantiate(GameManager.Instance.CurrentPlaySticker.Model,
                                hitPose.position, Quaternion.identity);
                            placedModelInstance.SetupFromStickerData(GameManager.Instance.CurrentPlaySticker);
                            placedModelInstance.StartRotatingWithTouch();
                            var cameraPos = camera.transform.position;
                            var targetLookAt = new Vector3(cameraPos.x, hitPose.position.y, cameraPos.z);
                            placedModelInstance.transform.LookAt(targetLookAt);
                            placedModelInstance.gameObject.AddComponent<Lean.Touch.LeanScale>();
                            var targetScale = placedModelInstance.GetRecommendedScaleToFitScreen(modelScaleFactor);
                            placedModelInstance.transform.localScale = targetScale;
                            placedModelInstance.GlobalMusicSource = musicSoundSource;
                        }
                        else
                        {
                            var modelTransform = placedModelInstance.transform;
                            modelTransform.position = hitPose.position;
                            var cameraPos = camera.transform.position;
                            var targetLookAt = new Vector3(cameraPos.x, hitPose.position.y, cameraPos.z);
                            modelTransform.LookAt(targetLookAt);
                            placedModelInstance.enabled = true;
                        }

                        messageBar.ShowMessage(rotateModelMessage);
                        tapToPlaceImage.gameObject.SetActive(false);
                        swipeToRotateImage.gameObject.SetActive(true);
                        doCheckForPlacement = false;
                        StopCoroutine(CheckForPlanePlacement());
                        buttonsCanvas.FadeIn();

                        // Prevent user from rotating model when placing it
                        StartCoroutine(EnableModelTouchControllsDelayed());
                    }
                }
            }
        }

        private IEnumerator EnableModelTouchControllsDelayed()
        {
            placedModelInstance.enabled = false;
            var touch = placedModelInstance.GetComponent<Lean.Touch.LeanScale>();
            touch.enabled = false;

            yield return new WaitForSeconds(1f);

            placedModelInstance.enabled = true;
            touch.enabled = true;
        }
    }
}