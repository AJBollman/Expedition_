
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Controls user interface elements. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public sealed class S_UserInterface : MonoBehaviour
{

    #region [Public]
    public bool startupMenuActive {
        get => startupMenu.activeInHierarchy;
        set {
            startupMenu.SetActive(value);
        }
    }
    public bool MainMenuActive {
        get => mainMenu.activeInHierarchy;
        set {
            mainMenu.SetActive(value);
        }
    }
    public bool pauseMenuActive {
        get => pauseMenu.activeInHierarchy;
        set {
            pauseMenu.SetActive(value);
            if(value == false) {
                foreach(GameObject g in codexThings) {
                    g.SetActive(false);
                }
            }
            if(!pauseMenu.activeInHierarchy && !optionsMenu.activeInHierarchy && !mainMenu.activeInHierarchy) {
                pauseMenuOpacityGoal = 0f;
                S_Player.cameraDrawAllowed = true;
            }
            else {
                pauseMenuOpacityGoal = 1f;
                S_Player.cameraDrawAllowed = false;
            }
        }
    }
    #endregion



    #region [Private]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject vignette;
    [SerializeField] private GameObject redlineVignette;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject startupMenu;
    [SerializeField] private GameObject background;
    [SerializeField] public GameObject loadingIndicator;
    [SerializeField] private List<GameObject> codexThings;
    private crosshairTypes currentCrosshair;
    private static float pauseMenuOpacityGoal;
    private static Image pauseMenuFill;
    private RectTransform loadingIndicatorRect;
    #endregion



    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    #region [Events]
    private void OnEnable()
    {
        instance = this;
        try {
            if(crosshair == null || vignette == null || redlineVignette == null || pauseMenu == null ||
            optionsMenu == null || mainMenu == null || item == null || startupMenu == null || loadingIndicator == null) {
                enabled = false;
                throw new System.Exception("All UI child gameObjects are required! Check the 'User Interface' in the inspector");
            }
            background.SetActive(true);
            pauseMenuFill = background.GetComponent<Image>();
            loadingIndicatorRect = loadingIndicator.GetComponent<RectTransform>();
            if(pauseMenuFill == null) throw new System.Exception("Pause Menu Background image missing.");
            startupMenuActive = true;

            isReady = true;
        }
        catch(Exception e) {
            enabled = false;
            isReady = false;
            Debug.LogException(e);
        }
    }

    private void Update() {
        pauseMenuFill.fillAmount = Mathf.Lerp(pauseMenuFill.fillAmount, pauseMenuOpacityGoal, Time.fixedUnscaledDeltaTime * 4f);
        if(loadingIndicator.activeInHierarchy) {
            loadingIndicatorRect.Rotate( new Vector3( 0, 0, loadingIndicatorRect.rotation.z + (30 * Time.unscaledDeltaTime) ) );
        }
    }
    #endregion



    #region [Methods]
    public static void SetCursor(crosshairTypes type)
    {
        //if ((int)type != (int)currentCrosshair) return;
        //Debug.Log("called");

        /*switch (type)
        {
            case crosshairTypes.draw:
                {
                    crosshair.GetComponent<Text>().text = "O";
                    break;
                }
            case crosshairTypes.yeet:
                {
                    crosshair.GetComponent<Text>().text = "v";
                    break;
                }
            case crosshairTypes.drop:
                {
                    crosshair.GetComponent<Text>().text = "v";
                    break;
                }
            case crosshairTypes.grab:
                {
                    crosshair.GetComponent<Text>().text = "> <";
                    break;
                }
            case crosshairTypes.place:
                {
                    crosshair.GetComponent<Text>().text = "> <";
                    break;
                }
            case crosshairTypes.nope:
                {
                    crosshair.GetComponent<Text>().text = "x";
                    break;
                }
            case crosshairTypes.none:
                {
                    crosshair.GetComponent<Text>().text = "";
                    break;
                }
            default:
            {
                break;
            }
        }
        currentCrosshair = type;*/
    }
    #endregion



    public static S_UserInterface instance { get; private set; }
    public bool isReady { get; private set;}
}
