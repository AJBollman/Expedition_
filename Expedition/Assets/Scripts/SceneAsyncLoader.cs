using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAsyncLoader : MonoBehaviour
{
    private bool loaderIsRunning;
    public KeyCode testingKey;
    public string testScene;

    private void Awake()
    {
        // Load data very slowly and try not to affect performance of the game.
        // Good for loading in the background while the game is playing.
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    void Update()
    {
        if (Input.GetKeyDown(testingKey))
        {
            startSceneLoad(testScene);
        }
    }

    IEnumerator LoadYourAsyncScene(string scene)
    {
        loaderIsRunning = true;
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            loaderIsRunning = false;
            yield return null;
        }
    }


    public void startSceneLoad(string scene)
    {
        if(loaderIsRunning)
        {
            Debug.LogWarning("Can't start loader, already loading!");
        }
        // Use a coroutine to load the Scene in the background
        else StartCoroutine(LoadYourAsyncScene(scene));
    }


}