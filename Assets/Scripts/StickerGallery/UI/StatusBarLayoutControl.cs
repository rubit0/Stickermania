using System.Collections.Generic;
using System.Linq;
using Domain;
using Manager;
using UnityEngine;

namespace StickerGallery.UI
{
    public class StatusBarLayoutControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject statusBarGridView;
        [SerializeField]
        private StickerStatusBox statusBoxPrefab;

        private void Start()
        {
            Populate(GameManager.Instance.CollectedStickers, GameManager.Instance.TrackableStickers, statusBoxPrefab);
        }

        private void Populate(List<CollectableSticker> collectedStickers, List<TrackableSticker> trackableStickers, StickerStatusBox prefab)
        {
            // Instante boxed for all collected stickers
            foreach (var collectable in collectedStickers)
            {
                var instance = Instantiate(prefab, statusBarGridView.transform);
                instance.UpdateStatus(collectable.StickerNumber, true);
            }

            // Instantiate remaining boxed for non-collected stickers
            foreach (var sticker in trackableStickers)
            {
                // Skip if sticker was already collected
                if (collectedStickers.Any(cs => cs.StickerNumber == sticker.StickerNumber))
                {
                    continue;
                }

                var instance = Instantiate(prefab, statusBarGridView.transform);
                instance.UpdateStatus(sticker.StickerNumber, false);
            }
        }
    }
}
