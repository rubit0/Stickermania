using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelfieLoadingScreen : MonoBehaviour
{
    [SerializeField]
    private string selfieSceneName;
    [SerializeField]
    private GameObject avatar;
    [SerializeField]
    private float delay = 2f;

    private void Awake()
    {
        StartCoroutine(IsLetterbox() 
            ? AnimateLoadingScreenCoroutineLetterbox() 
            : AnimateLoadingScreenCoroutine());
    }

    private IEnumerator AnimateLoadingScreenCoroutine()
    {
        avatar.gameObject.SetActive(false);
        Screen.orientation = ScreenOrientation.Portrait;
        yield return new WaitForSeconds(0.5f);
        avatar.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(selfieSceneName, LoadSceneMode.Single);
    }

    private IEnumerator AnimateLoadingScreenCoroutineLetterbox()
    {
        avatar.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        avatar.gameObject.SetActive(false);
        Screen.orientation = ScreenOrientation.Portrait;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(selfieSceneName, LoadSceneMode.Single);
    }

    private bool IsLetterbox()
    {
        return Screen.width / (float)Screen.height < 1.45f;
    }
}
