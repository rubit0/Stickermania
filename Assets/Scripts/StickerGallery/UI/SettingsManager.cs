using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StickerGallery.UI
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private string introSceneName;

        [Header("Sprites")]
        [SerializeField]
        private Sprite toggleOnSprite;
        [SerializeField]
        private Sprite toggleOffSprite;

        [Header("Icon references")]
        [SerializeField]
        private Image soundIcon;

        private void OnEnable()
        {
            // Set sound icons
            soundIcon.sprite = GameManager.Instance.SoundEffectsOn ? toggleOnSprite : toggleOffSprite;
        }

        public void ToggleSoundEffects()
        {
            GameManager.Instance.ToggleSoundEffects(!GameManager.Instance.SoundEffectsOn);
            soundIcon.sprite = GameManager.Instance.SoundEffectsOn ? toggleOnSprite : toggleOffSprite;
        }

        public void GoToIntroScene()
        {
            SceneManager.LoadScene(introSceneName);
        }
    }
}
