using System.Collections;
using System.Linq;
using Manager;
using UI.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XZImg;

namespace Scanning
{
    public class ScanSceneManager : MonoBehaviour
    {
        #region Inspector
        [Header("Main Settings")]
        [SerializeField]
        private string mainSceneName;
        [SerializeField]
        private string arSceneName;
        [SerializeField]
        private string mazeSceneName;
        [SerializeField]
        private StickerDetector stickerDetector;
        [SerializeField]
        private int failedScansUntilHelp = 3;
        [SerializeField]
        private float secondsUntilShowHelp = 10f;
        [SerializeField]
        private float delayUntilLoadARScene = 5f;

        [Header("Notification Bar")]
        [SerializeField]
        private GameObject notificationBarPanel;
        [SerializeField]
        private Text notificationBarTextField;
        [SerializeField]
        private string initialHintMessage;
        [SerializeField]
        private string stickerAlreadyScannedMessage;

        [Header("UI - Canvas")]
        [SerializeField]
        private CanvasGroupFade mainCanvas;
        [SerializeField]
        private CanvasGroupFade successCanvas;
        [SerializeField]
        private CanvasGroupFade unlockedCanvas;
        [SerializeField]
        private CanvasGroupFade helpCanvas;

        [Header("UI - Elements")]
        [SerializeField]
        private BounceHintButton helpButton;
        [SerializeField]
        private Text bottomTextMessage;
        [SerializeField]
        private Text successMessage;
        [SerializeField]
        private Text remainigStickersMessage;
        [SerializeField]
        private Text stickerNumberCounter;
        #endregion

        private int failedScanAttemps;
        private bool checkIfShouldshowHint;
        private bool isPresentingUnlockMessage;

        private void Start()
        {
            // Arange ui elements
            mainCanvas.gameObject.SetActive(true);
            mainCanvas.FadeIn();
            successCanvas.gameObject.SetActive(false);
            helpCanvas.gameObject.SetActive(false);

            // Subscribe to events
            stickerDetector.OnStickerDetected += HandleOnStickerDetected;
            stickerDetector.OnScanStateUpdated += HandleOnScanStateUpdated;

            ShowNotification(initialHintMessage);

            StartCoroutine(CheckIfShouldShowHintCoroutine());
        }

        private void OnDisable()
        {
            stickerDetector.enabled = false;
            checkIfShouldshowHint = false;
            StopCoroutine(CheckIfShouldShowHintCoroutine());
            stickerDetector.OnStickerDetected -= HandleOnStickerDetected;
            stickerDetector.OnScanStateUpdated -= HandleOnScanStateUpdated;
        }

        public void GoBackToMainScene()
        {
            SceneManager.LoadScene(mainSceneName);
        }

        public void GoToArScene()
        {
            if (GameManager.Instance.CurrentPlaySticker == null)
            {
                return;
            }

            if(isPresentingUnlockMessage)
            {
                StopAllCoroutines();
                isPresentingUnlockMessage = false;
                successCanvas.gameObject.SetActive(false);
                unlockedCanvas.gameObject.SetActive(true);
                StartCoroutine(AutoStartARSceneCoroutine());
                return;
            }

            StopAllCoroutines();
            GameManager.SuppressArHintMessage = true;
            SceneManager.LoadScene(arSceneName);
        }

        public void GoToMazeScene()
        {
            SceneManager.LoadScene(mazeSceneName);
        }

        public void StopCheckingShowHint()
        {
            checkIfShouldshowHint = false;
            StopCoroutine(CheckIfShouldShowHintCoroutine());
            failedScanAttemps = 0;
        }

        private void HandleOnStickerDetected(object sender, int stickerNumber)
        {
            Debug.Log("HandleOnStickerDetected " + stickerNumber);
            if (GameManager.Instance.TryClaimSticker(stickerNumber))
            {
                Debug.Log("TryClaimSticker - success");
                // Unsubscribe from events to prevent multiple triggers
                stickerDetector.OnStickerDetected -= HandleOnStickerDetected;
                stickerDetector.OnScanStateUpdated -= HandleOnScanStateUpdated;

                // Arrange ui elements
                mainCanvas.gameObject.SetActive(false);
                helpCanvas.gameObject.SetActive(false);

                // Get data and set text messages
                var animalData = GameManager.Instance
                    .TrackableStickers
                    .SingleOrDefault(ts => ts.StickerNumber == stickerNumber);

                // Set the model to be used in AR or 360 scene
                GameManager.Instance.CurrentPlaySticker = animalData;

                var accusativeAdjective = animalData.AccusativeAdjective.ToUpper();
                var accusativeName = animalData.AccusativeName.Replace("ß", "SS").ToUpper();
                successMessage.text = $"GESCHAFFT!\nDU HAST {accusativeAdjective}\n{accusativeName} GESAMMELT!";
                stickerNumberCounter.text = stickerNumber.ToString();

                var collectedStickersAmount = GameManager.Instance.CollectedStickers.Count();

                if (collectedStickersAmount > 12)
                {
                    successCanvas.gameObject.SetActive(true);
                    remainigStickersMessage.gameObject.SetActive(false);

                    StartCoroutine(AutoStartARSceneCoroutine());
                }
                else if (collectedStickersAmount == 12)
                {
                    // Maze scene has been unlocked
                    isPresentingUnlockMessage = true;
                    successCanvas.gameObject.SetActive(true);
                    remainigStickersMessage.gameObject.SetActive(false);

                    StartCoroutine(AutoOpenSuccessCanvas());
                }
                else if (collectedStickersAmount == 11)
                {
                    successCanvas.gameObject.SetActive(true);
                    remainigStickersMessage.text = "Dir fehlt noch 1 Sticker,\num das Labyrinth freizuschalten.";

                    StartCoroutine(AutoStartARSceneCoroutine());
                }
                else
                {
                    successCanvas.gameObject.SetActive(true);
                    var remaining = 12 - collectedStickersAmount;
                    remainigStickersMessage.text = $"Dir fehlen noch {remaining} Sticker,\num das Labyrinth freizuschalten.";

                    StartCoroutine(AutoStartARSceneCoroutine());
                }
            }
            else
            {
                ShowNotification(stickerAlreadyScannedMessage);
            }
        }

        private void ShowNotification(string text)
        {
            notificationBarTextField.text = text;
            notificationBarPanel.SetActive(true);
        }

        private void HandleOnScanStateUpdated(object sender, ScanState scanState)
        {
            // Show the help hint whenever the sticker failed
            // to be detected several times or some time has passed
            if (scanState == ScanState.Lost)
            {
                failedScanAttemps++;

                // too many attemps - show hint
                if (failedScanAttemps == failedScansUntilHelp)
                {
                    failedScanAttemps = 0;
                    helpButton.StartHintAnimation();

                    // Stop the time-check since hint is now displayed
                    checkIfShouldshowHint = false;
                    StopCoroutine(CheckIfShouldShowHintCoroutine());
                }
                else if (!checkIfShouldshowHint)
                {
                    StartCoroutine(CheckIfShouldShowHintCoroutine());
                }

                return;
            }

            // Stop showing hints when a sticker has been detected
            if (scanState == ScanState.Detected)
            {
                helpButton.StopHintAnimation();
                checkIfShouldshowHint = false;
                StopCoroutine(CheckIfShouldShowHintCoroutine());
            }
        }

        private IEnumerator CheckIfShouldShowHintCoroutine()
        {
            checkIfShouldshowHint = true;
            var waitTime = 0.5f;
            var waiter = new WaitForSeconds(waitTime);
            var timeToWait = secondsUntilShowHelp;

            while (checkIfShouldshowHint)
            {
                yield return waiter;
                timeToWait -= waitTime;
                // time has run out - show hint
                if(timeToWait <= 0f)
                {
                    helpButton.StartHintAnimation();
                    checkIfShouldshowHint = false;
                    break;
                }
            }
        }

        private IEnumerator AutoStartARSceneCoroutine()
        {
            yield return new WaitForSeconds(delayUntilLoadARScene);
            GoToArScene();
        }

        private IEnumerator AutoOpenSuccessCanvas()
        {
            yield return new WaitForSeconds(delayUntilLoadARScene);
            isPresentingUnlockMessage = false;
            successCanvas.FadeOut();
            unlockedCanvas.gameObject.SetActive(true);
            StartCoroutine(AutoStartARSceneCoroutine());
        }
    }
}
