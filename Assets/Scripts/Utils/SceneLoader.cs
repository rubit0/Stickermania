using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utils
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string sceneToLoad;

        private bool isLoadingScene;

        public void LoadScene()
        {
            if (isLoadingScene)
            {
                return;
            }

            isLoadingScene = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
