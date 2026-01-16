using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using Utils;
using System;
using SA.CrossPlatform.App;
using SA.CrossPlatform.UI;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public event EventHandler<bool> SuspendInteractions;

        #region Inspector
        [SerializeField]
        private AudioMixer masterAudioMix;
        [SerializeField]
        private string mainScene;
        [SerializeField]
        private string introScene;
        [SerializeField]
        private string debugScene;
        [SerializeField]
        private bool debugMode;
        [SerializeField]
        private TrackableSticker[] trackableStickers;
        [SerializeField]
        private AudioSource audioSource;
        #endregion

        public static GameManager Instance { get { return s_Instance; } }
        protected static GameManager s_Instance;

        public static bool SuppressArHintMessage { get; set; }

        /// <summary>
        /// All stickers raw data
        /// </summary>
        public List<TrackableSticker> TrackableStickers { get; private set; }

        /// <summary>
        /// All collected stickers by the user
        /// </summary>
        public List<CollectableSticker> CollectedStickers => playerData.collectedStickers;

        /// <summary>
        /// Model which is used to interact with in the AR and 360 Scene
        /// </summary>
        public TrackableSticker CurrentPlaySticker { get; set; }

        /// <summary>
        /// All sticker numbers which are not collected
        /// </summary>
        public List<int> MissingStickers => playerData.missingStickers;

        /// <summary>
        /// Has the player seen the initial intro.
        /// </summary>
        public bool PlayerHasSeenIntro => playerData.hasFinishedIntro;

        /// <summary>
        /// Should sound effects be played.
        /// </summary>
        public bool SoundEffectsOn => playerData.soundEffectsOn;

        /// <summary>
        /// Has treasure from the maze game been collected?
        /// </summary>
        public bool HasCollectedTreasure => playerData.hasCollectedTreassure;

        public bool HasDisplayedFirstTimeARUpdatePrompt { get; set; }

        public bool PlayerFirstTimeCheckingAR => playerData.playerUsesARFirstTime;

        public int AppStartAmount
        {
            get => playerData.appStartAmount;
            set
            {
                playerData.appStartAmount = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        public int DisplayedAnimalVisualizationsAmount
        {
            get => playerData.displayedAnimalVisualizationsAmount;
            set
            {
                playerData.displayedAnimalVisualizationsAmount = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        public bool AppRatingFlowDisabled
        {
            get => playerData.appRatingFlowDisabled;
            set
            {
                playerData.appRatingFlowDisabled = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        public bool Displayed2ndRatingDialogue
        {
            get => playerData.displayed2ndRatingDialogue;
            set
            {
                playerData.displayed2ndRatingDialogue = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        public bool Displayed8thRatingDialogue
        {
            get => playerData.displayed8thRatingDialogue;
            set
            {
                playerData.displayed8thRatingDialogue = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        public bool Displayed16thRatingDialogue
        {
            get => playerData.displayed16thRatingDialogue;
            set
            {
                playerData.displayed16thRatingDialogue = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        public bool DisplayedSelfieRatingDialogue
        {
            get => playerData.displayedAfterSelfieRatingDialogue;
            set
            {
                playerData.displayedAfterSelfieRatingDialogue = value;
                Persistence.SavePlayerData(playerData);
            }
        }

        /// <summary>
        /// Is the device running this app has AR support?
        /// </summary>
        public bool HasARSupport { get; set; }

        /// <summary>
        /// Forced fallback mode
        /// </summary>
        public bool HasFallbackModeForced { get; private set; }
        
        /// <summary>
        /// Indicates the user has declined the camera usage and should not be asked again.
        /// </summary>
        public bool UserDeclinedCameraUsage { get; set; }

        /// <summary>
        /// Use this indicator to show alternative dialogues to get permission
        /// </summary>
        public bool AskedUserFirstTimeForCameraPermission => playerData.hasAskedUserForCameraPermission;
        
        private PlayerData playerData;

        private void Start()
        {
            s_Instance = this;
            DontDestroyOnLoad(this.gameObject);

            StartCoroutine(CheckArAvailability());
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            TrackableStickers = trackableStickers.ToList();
            TrackableStickers.Sort();

            playerData = Persistence.LoadPlayerData(TrackableSticker.GetStickerNumbers(TrackableStickers));
            ToggleSound(SoundEffectsOn);
        }

        private IEnumerator CheckArAvailability()
        {
            while (ARSession.state == ARSessionState.None ||
                   ARSession.state == ARSessionState.Installing ||
                   ARSession.state == ARSessionState.CheckingAvailability)
            {
                yield return null;
            }

            if (ARSession.state == ARSessionState.NeedsInstall)
            {
                yield return ARSession.Install();
                this.HasARSupport = ARSession.state != ARSessionState.NeedsInstall;
            }
            else
            {
                this.HasARSupport = ARSession.state != ARSessionState.Unsupported;
            }
        }

        private void ToggleSound(bool state)
        {
            if (state)
            {
                masterAudioMix.SetFloat("Volume", 0f);
            }
            else
            {
                masterAudioMix.SetFloat("Volume", -80f);
            }
        }

        public void TakeScreenshot(Action onDone = null)
        {
            SuspendInteractions?.Invoke(this, true);
            audioSource.Play();
            var gallery = UM_Application.GalleryService;
            try
            {
                gallery.SaveScreenshot(DateTime.Now.ToString("yyyyMMddHHmmss"), (result) =>
                {
                    SuspendInteractions?.Invoke(this, false);
                    if (result.IsSucceeded)
                    {
                        Debug.Log("Saved");
                    }
                    else
                    {
                        Debug.Log("Failed: " + result.Error.FullMessage);
                    }
                    onDone?.Invoke();
                });
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                onDone?.Invoke();
            }
        }

        public void LoadMainScene()
        {
            string sceneToLoad;

            if (debugMode)
            {
                sceneToLoad = debugScene;
            }
            else
            {
                // Check if user has already seen the intro
                sceneToLoad = PlayerHasSeenIntro ? mainScene : introScene;
            }

            Debug.Log($"Loading [{sceneToLoad}] scene.");
            SceneManager.LoadScene(sceneToLoad);
        }

        public void SetHasFinishedIntro()
        {
            if (playerData.hasFinishedIntro)
            {
                return;
            }

            playerData.hasFinishedIntro = true;
            Persistence.SavePlayerData(playerData);
        }

        public void SetUserHasCheckedARFirstTime()
        {
            if (!playerData.playerUsesARFirstTime)
            {
                return;
            }

            playerData.playerUsesARFirstTime = false;
            Persistence.SavePlayerData(playerData);
        }

        public void ToggleSoundEffects(bool state)
        {
            if (playerData.soundEffectsOn == state)
            {
                return;
            }

            playerData.soundEffectsOn = state;
            Persistence.SavePlayerData(playerData);
            ToggleSound(state);
        }

        public void SetTreasureIsCollected()
        {
            if (playerData.hasCollectedTreassure)
            {
                return;
            }

            playerData.hasCollectedTreassure = true;
            Persistence.SavePlayerData(playerData);
        }

        public void SetAskedFirstTimeForCameraPermission()
        {
            if (playerData.hasAskedUserForCameraPermission)
            {
                return;
            }

            playerData.hasAskedUserForCameraPermission = true;
            Persistence.SavePlayerData(playerData);
        } 

        public bool TryClaimSticker(int detectedStickerNumber)
        {
            if (!playerData.IsStickerCollected(detectedStickerNumber))
            {
                playerData.missingStickers.Remove(detectedStickerNumber);
                var sticker = new CollectableSticker
                {
                    CollectedOrder = playerData.collectedStickers.Count + 1,
                    StickerNumber = detectedStickerNumber
                };
                playerData.collectedStickers.Add(sticker);
                Persistence.SavePlayerData(playerData);

                return true;
            }

            return false;
        }

        public void ForceFallbackMode()
        {
            this.HasFallbackModeForced = true;
        }

#if PLATFORM_ANDROID
        public static int GetAndroidApiLevel()
        {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                return version.GetStatic<int>("SDK_INT");
            }
        }

        public static void DisplayARWarningMessage(Action onComplete = null)
        {
            GameManager.Instance.SetUserHasCheckedARFirstTime();
            var cautionDialogue = new UM_NativeDialogBuilder("Hinweis", "Viel Spaß mit den witzigen 3D-Tieren. Aber Achtung:\n– Spiele mit ihnen auf einer freien, ebenen Fläche.\n– Pass auf, dass du deine Umgebung nicht aus den Augen verlierst.\n– Benutze die App unter Aufsicht von Erwachsenen.");
            cautionDialogue.SetNeutralButton("Ok", () => onComplete?.Invoke());
            cautionDialogue.Build().Show();
        }
#endif
    }
}
