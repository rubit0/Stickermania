using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain
{
    [Serializable]
    public class PlayerData
    {
        public bool soundEffectsOn = true;
        public bool hasFinishedIntro;
        public bool hasCollectedTreassure;
        public bool hasAskedUserForCameraPermission;
        public bool playerUsesARFirstTime = true;
        public List<CollectableSticker> collectedStickers;
        public List<int> missingStickers;
        public int displayedAnimalVisualizationsAmount;
        public int appStartAmount;

        // App rating data
        public bool appRatingFlowDisabled;
        public bool displayed2ndRatingDialogue;
        public bool displayed8thRatingDialogue;
        public bool displayed16thRatingDialogue;
        public bool displayedAfterSelfieRatingDialogue;

        public static PlayerData Create(List<int> allStickers)
        {
            return new PlayerData
            {
                collectedStickers = new List<CollectableSticker>(),
                missingStickers = allStickers
            };
        }

        public bool IsStickerCollected(int stickerNumber)
        {
            return !missingStickers.Contains(stickerNumber)
                   && collectedStickers.Any(s => s.StickerNumber == stickerNumber);
        }
    }
}
