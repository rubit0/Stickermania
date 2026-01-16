using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Manager;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StickerGallery.UI
{
    public class PopulateStickersGrid : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField]
        private int nrOfCols = 3;
        [SerializeField]
        private float stickerMargin = 21f;
        [SerializeField]
        private float settingsMenuOffset = 24f;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject scanModeButtonPrefab;
        [FormerlySerializedAs("stickerPrefab")] [SerializeField]
        private SelectableSticker selectableStickerPrefab;
        [SerializeField]
        private GameObject emptyStickerPrefab;
        [SerializeField]
        private GameObject allStickersScannedPrefab;
        [SerializeField]
        private GameObject oskarAvatar;
        [SerializeField]
        private RectTransform settingsPanel;

        private void Start()
        {
            SetupGrid();
            Populate(GameManager.Instance.CollectedStickers, scanModeButtonPrefab, selectableStickerPrefab, emptyStickerPrefab, oskarAvatar);
        }

        private void SetupGrid()
        {
            var rect = GetComponent<RectTransform>().rect;
            var contentWidth = rect.width;
            var imgWidth = Mathf.Round((contentWidth - ((float)nrOfCols + 1.0f) * stickerMargin) / (float)nrOfCols);

            var gridLayoutGroup = GetComponent<GridLayoutGroup>();
            gridLayoutGroup.cellSize = new Vector2(imgWidth, imgWidth);
        }

        public void Populate(List<CollectableSticker> collectedStickers, GameObject scanModeButton, SelectableSticker selectableStickerPrefab, GameObject emptyStickerPrefab, GameObject avatar)
        {
            // Instantiate collected stickers
            foreach (var collectable in collectedStickers)
            {
                var instance = Instantiate(selectableStickerPrefab, transform);
                var sticker = GameManager.Instance.TrackableStickers.SingleOrDefault(s => s.StickerNumber == collectable.StickerNumber);
                if (sticker != null)
                {
                    instance.SetupSticker(sticker);
                }
            }

            var remaining = GameManager.Instance.TrackableStickers.Count - collectedStickers.Count;
            if (remaining > 0)
            {
                // Instantiate scan button
                Instantiate(scanModeButton, transform);

                // Instantiate empty stickers
                for (int i = 0; i < remaining; i++)
                {
                    Instantiate(emptyStickerPrefab, transform);
                }
            }
            else
            {
                // All stickers collect - instantiate success sticker
                Instantiate(allStickersScannedPrefab, transform);
            }

            // Instantiate avatar
            Instantiate(avatar, transform);

            // And finally the settings panel
            StartCoroutine(AddSettingsPanelCoroutine());
        }

        private IEnumerator AddSettingsPanelCoroutine()
        {
            var parent = GetComponent<RectTransform>();
            var gridLayoutGroup = GetComponent<GridLayoutGroup>();
            gridLayoutGroup.padding.bottom += (int)(settingsPanel.sizeDelta.y + settingsMenuOffset);

            // Skip a frame to let UI layout be recalculated
            yield return null;

            var settingsPanelPosition = settingsPanel.localPosition;
            settingsPanelPosition = new Vector3(settingsPanelPosition.x, -parent.sizeDelta.y, settingsPanelPosition.z);
            settingsPanel.localPosition = settingsPanelPosition;
        }
    }
}
