using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private UnityEvent onHit;
    [SerializeField]
    private UnityEvent onPlayedMusic;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioMixerSnapshot defaultMix;
    [SerializeField]
    private AudioMixerSnapshot sfxMix;

    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            boxCollider.enabled = false;

            if(onHit.GetPersistentEventCount() > 0)
            {
                onHit.Invoke();
            }

            if(audioSource != null)
            {
                StartCoroutine(StartSoundFadeCoroutine());
            }
        }
    }

    private IEnumerator StartSoundFadeCoroutine()
    {
        var waiter = new WaitForSeconds(audioSource.clip.length);
        audioSource.Play();
        sfxMix.TransitionTo(0.25f);
        yield return waiter;
        defaultMix.TransitionTo(0.2f);

        if (onPlayedMusic.GetPersistentEventCount() > 0)
        {
            onPlayedMusic.Invoke();
        }

        Destroy(boxCollider);
        Destroy(this);
    }
}
