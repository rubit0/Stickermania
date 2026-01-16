using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Development
{
    public class DebugMenuManager : MonoBehaviour
    {
        #region Inspector
        [Header("Scene Names")]
        [SerializeField]
        private string mainSceneName;
        [SerializeField]
        private string introSceneName;
        [SerializeField]
        private string scanningSceneName;
        [SerializeField]
        private string modelViewerSceneName;
        [SerializeField]
        private string fallbackLabyrinthSceneName;
        [SerializeField]
        private string treasureRoomSceneName;

        [Header("Misc")]
        [SerializeField]
        private Button unlockStickerButton;
        [SerializeField]
        private Button unlockSelfieButton;
        #endregion

        private void Start()
        {
            if (GameManager.Instance.MissingStickers.Count < 1)
            {
                unlockStickerButton.interactable = false;
            }

            if (GameManager.Instance.HasCollectedTreasure)
            {
                unlockSelfieButton.interactable = false;
            }
        }

        public void LoadMainScene()
        {
            SceneManager.LoadScene(mainSceneName);
        }

        public void LoadModelViewer()
        {
            SceneManager.LoadScene(modelViewerSceneName);
        }

        public void LoadIntroScene()
        {
            SceneManager.LoadScene(introSceneName);
        }

        public void LoadScanningScene()
        {
            SceneManager.LoadScene(scanningSceneName);
        }

        public void LoadFallbackLabyrinth()
        {
            SceneManager.LoadScene(fallbackLabyrinthSceneName);
        }

        public void LoadTreasureRoom()
        {
            SceneManager.LoadScene(treasureRoomSceneName);
        }

        public void ClearData()
        {
            Persistence.ClearPlayerData();
            SceneManager.LoadScene(0);
        }

        public void UnlockSticker()
        {
            if(GameManager.Instance.MissingStickers.Count < 1)
            {
                UnityEngine.Debug.Log("All Stickers have been already unlocked");
                return;
            }

            var nextSicker = GameManager.Instance.MissingStickers[0];
            var success = GameManager.Instance.TryClaimSticker(nextSicker);
            if (success && GameManager.Instance.MissingStickers.Count < 1)
            {
                unlockStickerButton.interactable = false;
            }

            UnityEngine.Debug.Log($"Sticker nr{nextSicker} has been unlocked: {success}");
        }

        public void UnlockSelfieMode()
        {
            if (GameManager.Instance.HasCollectedTreasure)
            {
                return;
            }

            GameManager.Instance.SetTreasureIsCollected();
            unlockSelfieButton.interactable = false;
        }

        public void ForceFallbackMode()
        {
            GameManager.Instance.ForceFallbackMode();
        }
    }
}
