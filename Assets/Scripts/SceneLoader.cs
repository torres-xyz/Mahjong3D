using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Canvas loadingCanvas;
    public static EventHandler<string> SceneFinishedLoading;

    private void OnEnable()
    {
        loadingCanvas.sortingOrder = 100;
        StartGameButton.LoadMainScene += OnLoadMainScene;
    }
    private void OnDisable()
    {
        StartGameButton.LoadMainScene += OnLoadMainScene;
    }

    private void OnLoadMainScene(object sender, EventArgs e)
    {
        SceneManager.UnloadSceneAsync("IntroScene");
        StartCoroutine(WaitForLoadSceneAsync("MainScene"));
    }

    void Start()
    {
#if UNITY_EDITOR
        //return;
#endif

        //Only load intro scene at start if the Base Scene is the active scene.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneAt(0))
            StartCoroutine(WaitForLoadSceneAsync("IntroScene"));
    }

    IEnumerator WaitForLoadSceneAsync(string sceneName)
    {
        loadingCanvas.enabled = true;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        loadingCanvas.enabled = false;
        SceneFinishedLoading?.Invoke(this, sceneName);
    }
}
