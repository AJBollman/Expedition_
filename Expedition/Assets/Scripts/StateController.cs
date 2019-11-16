
// This is basically a big wrapper for an enum called gameStates.
//      StateController.getState();
//      StateController.setState(gameStates.yourstatehere);

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StateController : MonoBehaviour
{
    public bool autoStartGameInEditor;
    private static gameStates state = gameStates.normal;
    public static Region activeRegion;
    public static Camera activeRegionCamera;
    public static Portal activePortal;
    public static CameraOperator cam;

    private static GameObject camInitialLookAt;
    private static GameObject camInitialFollowPoint;
    private static StateController inst;
    private static GameObject transitionDinghy;
    private static bool canStartup;

    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
		else
		{
			DontDestroyOnLoad(gameObject);
			StaticsList.add(gameObject);
		}
		inst = this;
    }

    private void Start()
    {
        cam = GameObject.Find("CameraContainer").GetComponent<CameraOperator>(); if (!cam) throw new System.Exception("Camera Container not found. Make sure there is a Camera Container");
        if (SceneManager.GetActiveScene().name == "World")
        {
            canStartup = true;
            camInitialLookAt = cam.lookAt;
            camInitialFollowPoint = cam.followPoint;
            setState(gameStates.menu);
            transitionDinghy = GameObject.Find("dinghy");
            transitionDinghy.SetActive(false);
        }
        else canStartup = false;
        if (Application.isEditor && autoStartGameInEditor) StartGame();
    }



    public static void setState(gameStates state)
    {
        //Debug.Log("Game state changed to "+state);
        StateController.state = state;

        switch(StateController.state)
        {
            case gameStates.normal: {
                    cam.defaultFOV = 90f;
                    cam.enableControls = true;
                    cam.enableLookAt = false;
                    cam.followPoint = GameObject.Find("The Explorer");
                    Camera.main.nearClipPlane = 0.01f;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    cam.smoothTime = 32;
                    Time.timeScale = 1f;
                    //cam.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 85f, 0));
                    break;
            }
            case gameStates.menu:
            {
                    cam.defaultFOV = 30f;
                    cam.enableControls = false;
                    cam.enableLookAt = true;
                    cam.lookAt = camInitialLookAt;
                    cam.followPoint = camInitialFollowPoint;
                    Camera.main.GetComponent<Animator>().enabled = true;
                    Camera.main.nearClipPlane = 1;
                    cam.smoothTime = 2;
                    UserInterface.SetCursor(crosshairTypes.none);
                    break;
            }
            case gameStates.paused:
            {
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    cam.enableControls = false;
                    break;
            }
            case gameStates.redline:
            {
                    Time.timeScale = 0.5f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    cam.enableControls = false;
                    break;
            }
        }
    }


    private static IEnumerator flyToStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        setState(gameStates.normal);
        Destroy(GameObject.Find("HideAfterTransition"));
        transitionDinghy.SetActive(true);
        Camera.main.GetComponent<Animator>().enabled = false;
        inst.StartCoroutine(camr(0.1f));
    }
    private static IEnumerator camr(float delay)
    {
        yield return new WaitForSeconds(delay);
        cam.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 85f, 0));
    }



    public static void StartGame()
    {
        if (!canStartup) return;
        GameObject mm = GameObject.Find("MainMenu");
        if (mm != null) mm.SetActive(false);
		GameObject bo = GameObject.Find("S_Boat");
        GameObject ex = GameObject.Find("The Explorer");
        GameObject.Find("Logo").SetActive(false);
        cam.followPoint = ex;
        cam.defaultFOV = 90f;
        cam.lookAt = bo;
        cam.smoothTime = 0.75f;
		ex.transform.LookAt(bo.transform);
		inst.StartCoroutine(flyToStart(Application.isEditor ? 0.1f : 5f));
    }



    public static gameStates getState()
    {
        return StateController.state;
    }



    public static GameObject getTraveller()
    {
        return GameObject.Find("Traveller");
    }

    public static void startTraveller()
    {
        var trav = StateController.getTraveller().GetComponent<NavMeshMovement>();
        trav.spawn(activePortal.transform.position);
        trav.givePath(activeRegion.getLatestRedLine());
        trav.navMove();
    }
}

// Menu: In the main menu of the game.
// Paused: Game is paused.
// Normal: Explorer can move around and do stuff.
// Fullmap: We are in fullscreen map mode; no movement allowed.
// Guiding: TODO not sure about this one.
public enum gameStates { menu, paused, normal, redline };