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
    [SerializeField] private bool emergency;
    [SerializeField] private Vector3 aihf;
    [SerializeField] private BiomeScene ABlue;
    [SerializeField] private BiomeScene BOrange;
    private bool _isInside;
    private bool _isA;
    private BoxCollider _ACollider;
    private BoxCollider _BCollider;
    #endregion



    #region [Events]
    void Awake() {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        _ACollider = transform.Find("SCENE A TRANSIT").GetComponent<BoxCollider>();
        _BCollider = transform.Find("SCENE B TRANSIT").GetComponent<BoxCollider>();
    }
    #endregion


    #region [Methods]
    public void OnDetect(bool isA) {
       
    }

    public void OnExit(bool isA) {
        _isA = isA;
        if (_isA) {
            if(_isInside) { // entered from A
                _ACollider.isTrigger = false;
                _BCollider.isTrigger = false;
                startSceneLoad(ABlue, false);
            }
            else { // exited from A
                _BCollider.isTrigger = true;
                startSceneLoad(ABlue, true);
            }
        }
        else {
            if (_isInside) { // entered from B 
                _ACollider.isTrigger = false;
                _BCollider.isTrigger = false;
                startSceneLoad(BOrange, false);
            }
            else { // exited from B
                _ACollider.isTrigger = true;
                startSceneLoad(BOrange, true);
            }
        }
    }

    // Entered middle collider.
    public void OnMid(bool isIn) {
        _isInside = isIn;
    }

    public IEnumerator LoadAsyncScene(int sceneIndex) {

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
                    if(emergency) {
                        Expedition.Player.gameObject.transform.position = aihf;
                        Expedition.EnterTitleScreenState();
                    }
                }
                catch(Exception e){Debug.LogException(e);}
                if(!_isA) {_ACollider.isTrigger = true;}
                else _BCollider.isTrigger = true;
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
