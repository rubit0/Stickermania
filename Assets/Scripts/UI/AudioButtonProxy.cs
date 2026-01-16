using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Button))]
    public class AudioButtonProxy : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private UnityEvent onAudioPlayStarted;
        [SerializeField]
        private UnityEvent onAudioPlayFinished;

        private AudioSource audioSource;
        private Button button;
        private bool coroutineActive;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            button = GetComponent<Button>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (audioSource.clip == null || coroutineActive)
            {
                return;
            }

            StartCoroutine(ClickEventCoroutine());
        }

        private IEnumerator ClickEventCoroutine()
        {
            coroutineActive = true;
            onAudioPlayStarted?.Invoke();
            button.interactable = false;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
            button.interactable = true;
            coroutineActive = false;
            onAudioPlayFinished?.Invoke();
        }
    }
}
