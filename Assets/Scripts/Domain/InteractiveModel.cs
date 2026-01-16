using System.Collections;
using Lean.Touch;
using Manager;
using UnityEngine;
using UnityEngine.Events;

namespace Domain
{
    public class InteractiveModel : MonoBehaviour
    {
        public AudioSource GlobalMusicSource { get; set; }

        public AudioClip sfxAudio;
        public AudioClip musicAudio;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private Collider hitBox;
        [SerializeField]
        private Animation mainAnimationTarget;
        [SerializeField]
        private Animation[] subActionAnimationTargets;
        [SerializeField]
        private float rotationSpeed = 0.75f;
        [SerializeField]
        private UnityEvent onAnimationStarted;

        private Transform instanceTransform;
        private Vector2 lastTouchPosition;
        private AudioSource musicAudioSource;
        private bool isPlayingAnimation;
        private float scaleOffset;

        private void Start()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            audioSource.spatialBlend = 1f;
            GameManager.Instance.SuspendInteractions += OnSuspendInteractionsHandler;
        }

        private void OnDestroy()
        {
            GameManager.Instance.SuspendInteractions -= OnSuspendInteractionsHandler;
        }

        private void OnSuspendInteractionsHandler(object sender, bool state)
        {
             if(!isPlayingAnimation)
            {
                return;
            }

            if (state)
            {
                GlobalMusicSource.Pause();
                audioSource.Pause();
                mainAnimationTarget.enabled = false;
                foreach (var animationTarget in subActionAnimationTargets)
                {
                    animationTarget.enabled = false;
                }
            }
            else
            {
                GlobalMusicSource.UnPause();
                audioSource.UnPause();
                mainAnimationTarget.enabled = true;
                foreach (var animationTarget in subActionAnimationTargets)
                {
                    animationTarget.enabled = true;
                }
            }
        }

        private void OnEnable()
        {
            instanceTransform = transform;
            LeanTouch.OnFingerTap += OnFingerTap;
        }

        private void OnDisable()
        {
            LeanTouch.OnFingerTap -= OnFingerTap;
        }

        public void StartRotatingWithTouch()
        {
            StartCoroutine(CheckTouchForRotationCoroutine());
        }

        private void OnFingerTap(LeanFinger finger)
        {
            if (isPlayingAnimation || finger.IsOverGui)
            {
                return;
            }

            var ray = finger.GetRay();
            if (Physics.Raycast(ray, out var hitInfo))
            {
                if (hitInfo.collider == hitBox)
                {
                    onAnimationStarted?.Invoke();
                    StartCoroutine(PlayActionAnimationCoroutine());
                }
            }
        }

        private IEnumerator CheckTouchForRotationCoroutine()
        {
            while (gameObject.activeSelf)
            {
                if (Input.touchCount == 1)
                {
                    var touch = Input.GetTouch(0);

                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            // store initial touch position
                            lastTouchPosition = touch.position;
                            break;
                        case TouchPhase.Moved:
                            // get the moved difference and convert it to an angle
                            // using the rotationSpeed as sensibility
                            var rotationX = (touch.position.x - lastTouchPosition.x) * rotationSpeed;
                            instanceTransform.Rotate(Vector3.up, -rotationX, Space.World);
                            lastTouchPosition = touch.position;
                            break;
                    }
                }

                yield return null;
            }
        }

        private IEnumerator PlayActionAnimationCoroutine()
        {
            isPlayingAnimation = true;

            // Play sound and music
            if(audioSource != null && sfxAudio != null)
            {
                audioSource.clip = sfxAudio;
                audioSource.Play();
            }
            if(GlobalMusicSource != null && musicAudio != null)
            {
                GlobalMusicSource.clip = musicAudio;
                GlobalMusicSource.Play();
            }

            if (mainAnimationTarget != null)
            {
                mainAnimationTarget.Play("Take 001");
            }
            foreach (var animationTaget in subActionAnimationTargets)
            {
                animationTaget.Play("Take 001");
            }

            var audioClipDuration = audioSource.clip.length > GlobalMusicSource.clip.length 
                ? audioSource.clip.length 
                : GlobalMusicSource.clip.length;
            var animationClipDuration = mainAnimationTarget.clip.length;
            var waitTime = audioClipDuration > animationClipDuration
                ? audioClipDuration
                : animationClipDuration;

            yield return new WaitForSeconds(waitTime);
            isPlayingAnimation = false;
        }

        public void SetupFromStickerData(TrackableSticker trackableSticker)
        {
            sfxAudio = trackableSticker.effectSound;
            musicAudio = trackableSticker.music;
            scaleOffset = trackableSticker.ARViewScaleOffset;
        }

        public Vector3 GetRecommendedScaleToFitScreen(float offset = 1f)
        {
            var camera = Camera.main.transform;
            if (camera == null)
            {
                return transform.localScale;
            }

            var bounds = GetComponent<BoxCollider>().size;
            var size = bounds.y + bounds.x;
            var plane = new Plane(camera.forward, camera.position);
            var distance = plane.GetDistanceToPoint(transform.position);
            var delta = (distance / size) * offset;
            delta *= scaleOffset;

            Debug.Log("GetRecommendedScaleToFitScreen " + this.GetHashCode());


            return new Vector3(delta, delta, delta);
        }
    }
}
