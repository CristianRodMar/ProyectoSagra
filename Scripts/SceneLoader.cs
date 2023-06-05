using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{

    public GameObject loadingScreen;
    public Slider progressBar;
    public void LoadLevel (string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    public void LeaveGame()
    {
        Application.Quit();
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressBar.value = progress;
            yield return null;
        }
    }
}



