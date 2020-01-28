using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public string AScene;
    public string BScene;
    public string name;
    public GameObject associatedStuffToRemove;
    private bool isInside;
    private bool insideA;
    private bool insideB;
    private bool loaderIsRunning;
    public static List<string> names = new List<string>();

    void Awake()
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        if (names.Contains(name))
        {
            Debug.Log("Removing duplicate scene trasition");
            if(associatedStuffToRemove != null && GameObject.Find(associatedStuffToRemove.name))
            {
                //Destroy(associatedStuffToRemove);
            }
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            //StaticsList.add(gameObject);
        }
        names.Add(name);
    }

    public void OnDetect(bool isA)
    {
       
    }

    public void OnExit(bool isA)
    {
        if (isA)
        {
            if(isInside) // entered from A
            {
                Debug.Log("entered from B");
                startSceneLoad(AScene, false);
            }
            else // exited from A
            {
                Debug.Log("exited from B");
                startSceneLoad(AScene, true);
            }
        }
        else
        {
            if (isInside) // entered from B
            {
                Debug.Log("entered from A");
                startSceneLoad(BScene, false);
            }
            else // exited from B
            {
                Debug.Log("exited from A");
                startSceneLoad(BScene, true);
            }
        }
    }

    // Entered middle collider.
    public void OnMid(bool isIn)
    {
        isInside = isIn;
    }

    IEnumerator LoadYourAsyncScene(string scene)
    {
        if (Application.CanStreamedLevelBeLoaded(scene))
        {
            if(!SceneManager.GetSceneByName(scene).isLoaded)
            {
                //Debug.Log(scene += "akhbsd");
                loaderIsRunning = true;
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    loaderIsRunning = false;
                    //SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
                    yield return null;
                }
            }
            else Debug.Log("Scene '" + scene + "' is already loaded!");
        }
        else throw new Exception("Scene '" + scene + "' doesn't exist!");
    }

    IEnumerator UnloadAsyncScene(string scene)
    {
        if (Application.CanStreamedLevelBeLoaded(scene))
        {
            if (SceneManager.GetSceneByName(scene).isLoaded)
            {
                Debug.Log("HHERE");
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
                while (asyncUnload != null && !asyncUnload.isDone)
                {
                    yield return null;
                }
            }
            else Debug.Log("Tried to unload, but scene '" + scene + "' isn't loaded!");
        }
        else throw new Exception("Tried to unload, but scene '" + scene + "' doesn't exist!");
    }

    // Load scene in background.
    public void startSceneLoad(string scene, bool unload)
    {
        //Debug.Log(scene);
        if (loaderIsRunning)
        {
            Debug.LogWarning("Can't start loader, already loading!");
        }
        // NOT WORKING RIGHT NOW... /////////////////////////////////////////////////////////////////////////
        else if(unload) StartCoroutine(UnloadAsyncScene(scene));
        else StartCoroutine(LoadYourAsyncScene(scene));
    }
}
