using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[DisallowMultipleComponent]
public sealed class Transition : MonoBehaviour {
    #region [Static]
    public static List<string> names = new List<string>();
    public static bool loaderIsRunning { get; private set; }
    #endregion

    #region [Private]
    [SerializeField] private BiomeScene ABlue;
    [SerializeField] private BiomeScene BOrange;
    private bool _isInside;
    private bool _insideA;
    private bool _insideB;
    #endregion



    #region [Events]
    void Awake() {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
    }
    #endregion


    #region [Methods]
    public void OnDetect(bool isA) {
       
    }

    public void OnExit(bool isA) {
        if (isA) {
            if(_isInside) { // entered from A
                Debug.Log("entered from B");
                startSceneLoad(ABlue, false);
            }
            else { // exited from A
                Debug.Log("exited from B");
                startSceneLoad(ABlue, true);
            }
        }
        else {
            if (_isInside) { // entered from B 
                Debug.Log("entered from A");
                startSceneLoad(BOrange, false);
            }
            else { // exited from B
                Debug.Log("exited from A");
                startSceneLoad(BOrange, true);
            }
        }
    }

    // Entered middle collider.
    public void OnMid(bool isIn) {
        _isInside = isIn;
    }

    public static IEnumerator LoadAsyncScene(int sceneIndex) {

        if(!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded) {
            Expedition.BiomeTransition(sceneIndex);
            Expedition.UserInterface.loadingIndicator.SetActive(true);
            loaderIsRunning = true;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            asyncLoad.completed += (asyncOperation) => {
                loaderIsRunning = false;
                Expedition.UserInterface.loadingIndicator.SetActive(false);
                try {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
                }
                catch(Exception e){Debug.LogException(e);}
            };
            if (!asyncLoad.isDone) {
                loaderIsRunning = true;
                yield return null;
            }
        }
        else Debug.Log("Scene '" + sceneIndex + "' is already loaded!");
    }

    public static IEnumerator UnloadAsyncScene(int sceneIndex) {

        if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded) {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneIndex);
            while (asyncUnload != null && !asyncUnload.isDone) {
                yield return null;
            }
        }
        else Debug.Log("Tried to unload, but scene '" + sceneIndex + "' isn't loaded!");
    }

    public void startSceneLoad(BiomeScene scene, bool unload) {
        if (loaderIsRunning)
        {
            Debug.LogWarning("Can't start loader, already loading!");
        }
        else if(unload) StartCoroutine(UnloadAsyncScene((int)scene));
        else StartCoroutine(LoadAsyncScene((int)scene));
    }
    #endregion
}
