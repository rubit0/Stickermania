using System.Linq;
using Domain;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Development
{
    public class ModelViewerSceneManager : MonoBehaviour
    {
        [SerializeField]
        private float modelScaleOffset;
        [SerializeField]
        private string debugSceneName;
        [SerializeField]
        private Dropdown dropdown;
        [SerializeField]
        private Transform pivotPoint;

        private InteractiveModel[] animalInstances;
        private InteractiveModel current;

        private void Start()
        {
            animalInstances = GameManager.Instance
                .TrackableStickers
                .Where(st => st.Model != null)
                .Select(st =>
                {
                    var go = Instantiate(st.Model, pivotPoint);
                    go.gameObject.SetActive(false);
                    go.name = st.DisplayName;

                    return go;
                })
                .ToArray();

            animalInstances[0].gameObject.SetActive(true);
            current = animalInstances[0];
            dropdown.options = animalInstances.Select(ai => new Dropdown.OptionData(ai.name)).ToList();
            dropdown.onValueChanged.AddListener(HandleAnimalSelection);
        }

        private void OnDestroy()
        {
            dropdown.onValueChanged.RemoveListener(HandleAnimalSelection);
        }

        private void HandleAnimalSelection(int index)
        {
            current.gameObject.SetActive(false);
            current = animalInstances[index];
            current.gameObject.SetActive(true);
            current.transform.localScale = current.GetRecommendedScaleToFitScreen(modelScaleOffset);
        }

        public void GoToDebugScene()
        {
            SceneManager.LoadScene(debugSceneName);
        }
    }
}
