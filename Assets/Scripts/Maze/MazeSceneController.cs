using UnityEngine;
using UnityEngine.SceneManagement;

namespace Maze
{
    public class MazeSceneController : MonoBehaviour
    {
        [SerializeField]
        private string mainSceneName;
        [SerializeField]
        private string boNameToRemove = "Bo";
        [SerializeField]
        private GameObject canvas;

        private OskarMovementController oskarMovement;

        private void Start()
        {
            oskarMovement = FindObjectOfType<OskarMovementController>();
            if(oskarMovement != null)
            {

                oskarMovement.onEnemyHit.AddListener(() => HidePanels());
                oskarMovement.onEnemyExit.AddListener(() => ShowPanels());
            }

            var bo = GameObject.Find(boNameToRemove);
            if(bo != null)
            {
                Destroy(bo);
            }
        }

        private void HidePanels()
        {
            canvas.SetActive(false);
        }

        private void ShowPanels()
        {
            canvas.SetActive(true);
        }

        public void GoToMainScene()
        {
            SceneManager.LoadScene(mainSceneName);
        }
    }
}
