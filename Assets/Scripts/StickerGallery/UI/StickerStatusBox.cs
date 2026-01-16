using UnityEngine;
using UnityEngine.UI;

namespace StickerGallery.UI
{
    public class StickerStatusBox : MonoBehaviour
    {
        #region Inspector
        [SerializeField]
        private Sprite collectedImage;
        [SerializeField]
        private Sprite missingImage;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private Text counter;
        #endregion

        public void UpdateStatus(int nr, bool collected)
        {
            counter.text = nr.ToString();
            backgroundImage.sprite = collected ? collectedImage : missingImage;
        }

        public void UpdatePosition(Vector2 size, Vector2 position, float textWidth)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (rectTransform == null)
            {
                Debug.LogError("rectTrans is null");
                return;
            }

            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = position;

            counter.rectTransform.sizeDelta = new Vector2(textWidth, textWidth);
            counter.rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
