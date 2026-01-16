using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToggleSoundButton : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private string soundOnLabelMessage = "TON AN";
        [SerializeField]
        private string soundOffLabelMessage = "TON AUS";

        [Header("Sprites")]
        [SerializeField]
        private Sprite toggleOnSprite;
        [SerializeField]
        private Sprite toggleOffSprite;

        [Header("Icon reference")]
        [SerializeField]
        private Image soundIcon;
        [SerializeField]
        private Text textLabel;

        private void OnEnable()
        {
            Invalidate();
        }

        public void ToggleSoundEffects()
        {
            GameManager.Instance.ToggleSoundEffects(!GameManager.Instance.SoundEffectsOn);
            Invalidate();
        }

        public void Invalidate()
        {
            if (soundIcon != null)
            {
                soundIcon.sprite = GameManager.Instance.SoundEffectsOn ? toggleOnSprite : toggleOffSprite;
            }

            if (textLabel != null)
            {
                textLabel.text = GameManager.Instance.SoundEffectsOn ? soundOnLabelMessage : soundOffLabelMessage;
            }
        }
    }
}
