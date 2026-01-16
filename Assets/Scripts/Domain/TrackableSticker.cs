using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Domain
{
    [CreateAssetMenu(fileName = "Sticker", menuName = "Trackable Sticker", order = 0)]
    public class TrackableSticker : ScriptableObject, IComparable
    {
        public int StickerNumber;
        public float PhysicalWidth = 1.77165f;
        public string DisplayName;
        public string AccusativeAdjective;
        public string AccusativeName;
        public Sprite ThumbnailImage;
        public InteractiveModel Model;
        public AudioClip effectSound;
        public AudioClip music;
        public TextAsset ImageClassifier;
        [Range(0f, 2f)]
        public float ARViewScaleOffset = 1f;

        public bool IsValid()
        {
            return StickerNumber > 0
                   && !string.IsNullOrWhiteSpace(DisplayName)
                   && ThumbnailImage != null
                   && Model != null
                   && ImageClassifier != null;
        }

        public static List<int> GetStickerNumbers(List<TrackableSticker> stickers)
        {
            return stickers.Select(s => s.StickerNumber).ToList();
        }

        public int CompareTo(object obj)
        {
            if (obj is TrackableSticker other)
            {
                if (StickerNumber == other.StickerNumber)
                {
                    return 0;
                }

                return StickerNumber < other.StickerNumber ? 0 : 1;
            }

            return 0;
        }
    }
}