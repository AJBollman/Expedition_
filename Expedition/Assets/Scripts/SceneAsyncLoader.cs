using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAsyncLoader : MonoBehaviour
{
    private bool loaderIsRunning;

    private void Awake()
    {
        // Load data very slowly and try not to affect performance of the game.
        // Good for loading in the background while the game is playing.
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }

    IEnumerator LoadYourAsyncScene(string scene)
    {
        loaderIsRunning = true;
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.
        if (Application.CanStreamedLevelBeLoaded(scene))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                loaderIsRunning = false;
                yield return null;
            }
        }
        else throw new Exception("Scene '" + scene + "' doesn't exist!");
    }


    public void startSceneLoad(string scene)
    {
        Debug.Log(scene);
        if(loaderIsRunning)
        {
            Debug.LogWarning("Can't start loader, already loading!");
        }
        // Use a coroutine to load the Scene in the background
        else StartCoroutine(LoadYourAsyncScene(scene));
    }


}