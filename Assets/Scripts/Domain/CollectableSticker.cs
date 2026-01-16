using System;

namespace Domain
{
    [Serializable]
    public class CollectableSticker
    {
        public int StickerNumber { get; set; }
        public int CollectedOrder { get; set; }
    }
}
