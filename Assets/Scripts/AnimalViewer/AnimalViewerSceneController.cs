using Domain;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnimalViewer
{
    public class AnimalViewerSceneController : MonoBehaviour
    {
        [SerializeField]
        private bl_CameraOrbit camerOrbit;
        [SerializeField]
        private Transform cameraTarget;
        [SerializeField]
        private Transform spawnPoint;
        [SerializeField]
        private AudioSource musicSoundSource;
        [SerializeField]
        private string gallerySceneName = "MainScene";
        [SerializeField]
        private float modelScaleFactor = 2.5f;

        private InteractiveModel placedModelInstance;

        private void Start()
        {
            camerOrbit.isForMobile = !Application.isEditor;
            if (GameManager.Instance.CurrentPlaySticker != null)
            {
                PlaceModel();
            }
            GameManager.Instance.DisplayedAnimalVisualizationsAmount++;
        }

        private void PlaceModel()
        {
            placedModelInstance = Instantiate(GameManager.Instance.CurrentPlaySticker.Model, spawnPoint.position, Quaternion.identity);
            placedModelInstance.SetupFromStickerData(GameManager.Instance.CurrentPlaySticker);
            var targetScale = placedModelInstance.GetRecommendedScaleToFitScreen(modelScaleFactor);
            placedModelInstance.transform.localScale = targetScale;
            placedModelInstance.GlobalMusicSource = musicSoundSource;

            SetupCameraTarget();
            LookModelTowardsCamera();
        }

        public void GoToStickerGalleryScene()
        {
            SceneManager.LoadScene(gallerySceneName);
        }

        private void SetupCameraTarget()
        {
            var collider = placedModelInstance.GetComponent<BoxCollider>();
            if (collider == null)
            {
                return;
            }

            cameraTarget.position = placedModelInstance.transform.position + placedModelInstance.transform.TransformPoint(collider.center);
            camerOrbit.SetTarget(cameraTarget);
            camerOrbit.Vertical = 25;
        }

        private void LookModelTowardsCamera()
        {
            var cameraPos = camerOrbit.transform.position;
            var targetLookAt = new Vector3(cameraPos.x, 0f, cameraPos.z);
            placedModelInstance.transform.LookAt(targetLookAt);
        }
    }
}
