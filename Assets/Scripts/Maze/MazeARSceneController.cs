using System.Collections;
using System.Collections.Generic;
using UI.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class MazeARSceneController : MonoBehaviour
{
    [SerializeField]
    private GameObject mazePrefab;

    [Header("Misc")]
    [SerializeField]
    private string mainSceneName;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private ARVisualizationsController arVisualizationsController;
    [SerializeField]
    private ARRaycastManager raycastManager;
    [SerializeField]
    private AudioSource musicSoundSource;
    [SerializeField]
    [Tooltip("Time in seconds")]
    private float durationUntilHint;
    [SerializeField]
    private Material tapToPlaceMaterial;
    [SerializeField]
    private Material shadowPlaneMaterial;
    [SerializeField]
    private float modelScaleFactor = 0.25f;

    [Header("UI Elements")]
    [SerializeField]
    private CanvasGroup rootCanvasGroup;
    [SerializeField]
    private GameObject hintPlacingCanvas;
    [SerializeField]
    private GameObject hintGameCanvas;
    [SerializeField]
    private MessageBar messageBar;
    [SerializeField]
    private BounceHintButton hintButton;
    [SerializeField]
    private GameObject tapToPlaceImage;
    [SerializeField]
    private GameObject swipeToRotateImage;
    [SerializeField]
    private CanvasGroupFade buttonsCanvas;

    [Header("Messages")]
    [SerializeField]
    private string movePhoneMessage;
    [SerializeField]
    private string tapToPlaceMessage;

    private GameObject placedMaze;
    private bool checkShowingHint;
    private GameObject joystickCanvas;
    private OskarMovementController oskarMovement;

    private void Start()
    {
        joystickCanvas = GameObject.Find("Joystick Canvas");
        rootCanvasGroup.interactable = true;
        rootCanvasGroup.alpha = 1;
        buttonsCanvas.HideImmediately();
        StartCoroutine(CheckIfShouldShowHintCoroutine());
        StartCoroutine(CheckForInitialPlaneCoroutine(true));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void HidePanels()
    {
        rootCanvasGroup.gameObject.SetActive(false);
    }

    private void ShowPanels()
    {
        rootCanvasGroup.gameObject.SetActive(true);
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
        StartCoroutine(CheckForInitialPlaneCoroutine(false));
    }

    public void ShowHint()
    {
        rootCanvasGroup.gameObject.SetActive(false);
        if(joystickCanvas != null)
        {
            joystickCanvas.SetActive(false);
        }

        if (placedMaze != null)
        {
            hintGameCanvas.SetActive(true);
        }
        else
        {
            hintPlacingCanvas.SetActive(true);
        }
    }

    public void HideHint()
    {
        rootCanvasGroup.gameObject.SetActive(true);
        if (joystickCanvas != null)
        {
            joystickCanvas.SetActive(true);
        }
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
        messageBar.ShowMessage(movePhoneMessage, 25f);
        // Enable AR trackables visualization
        arVisualizationsController.ChangePointCloudVisualization(ARVisualizationsController.VisualizationMode.Show);
        if (withDelay)
        {
            arVisualizationsController.ChangePlaneVisualization(ARVisualizationsController.VisualizationMode.Invisible);
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
        messageBar.ShowMessage(tapToPlaceMessage, 15f);
        tapToPlaceImage.gameObject.SetActive(true);

        var doCheckForPlacement = true;
        while (doCheckForPlacement)
        {
            yield return null;

            // Check if screen has been touched
            if (Input.touchCount > 0)
            {
                // ... and it is the start of a touch
                var touchInfo = Input.GetTouch(0);
                if (touchInfo.phase != TouchPhase.Began)
                {
                    continue;
                }

                // Try raycast on a place to place the labyrinth
                var hitResults = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touchInfo.position, hitResults, TrackableType.PlaneWithinPolygon))
                {
                    // Disable AR trackables visualization
                    arVisualizationsController.ChangePointCloudVisualization(ARVisualizationsController.VisualizationMode.Invisible);
                    arVisualizationsController.ChangePlaneVisualization(ARVisualizationsController.VisualizationMode.Invisible);

                    // Handle placing and UI
                    var hitPose = hitResults[0].pose;

                    if(placedMaze == null)
                    {
                        placedMaze = Instantiate(mazePrefab, hitPose.position, Quaternion.identity);
                        var cameraPos = camera.transform.position;
                        var targetLookAt = new Vector3(cameraPos.x, hitPose.position.y, cameraPos.z);
                        placedMaze.transform.LookAt(targetLookAt);
                        placedMaze.transform.localRotation *= Quaternion.Euler(0, 180, 0);
                        placedMaze.gameObject.AddComponent<Lean.Touch.LeanScale>();
                        placedMaze.transform.localScale = Vector3.one * 0.045f;
                        oskarMovement = FindObjectOfType<OskarMovementController>();
                        if (oskarMovement != null)
                        {

                            oskarMovement.onEnemyHit.AddListener(() => HidePanels());
                            oskarMovement.onEnemyExit.AddListener(() => ShowPanels());
                        }
                    }
                    else
                    {
                        var modelTransform = placedMaze.transform;
                        modelTransform.position = hitPose.position;
                        var cameraPos = camera.transform.position;
                        var targetLookAt = new Vector3(cameraPos.x, hitPose.position.y, cameraPos.z);
                        modelTransform.LookAt(targetLookAt);
                        modelTransform.transform.localRotation *= Quaternion.Euler(0, 180, 0);
                        placedMaze.gameObject.SetActive(true);
                    }

                    messageBar.CleanUpAnimation();
                    tapToPlaceImage.gameObject.SetActive(false);
                    swipeToRotateImage.gameObject.SetActive(true);
                    doCheckForPlacement = false;
                    StopCoroutine(CheckForPlanePlacement());
                    StartCoroutine(EnableModelTouchControllsDelayed());
                    buttonsCanvas.FadeIn();
                }
            }
        }
    }

    private IEnumerator EnableModelTouchControllsDelayed()
    {
        var touch = placedMaze.GetComponent<Lean.Touch.LeanScale>();
        touch.enabled = false;

        yield return new WaitForSeconds(1f);

        touch.enabled = true;
    }
}
