
using UnityEngine;
using UnityEngine.UI;

public enum crosshairTypes { draw, yeet, drop, grab, place, nope, none };

/// <summary> Controls User Interface elements. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Canvas))]
public sealed class UserInterface : MonoBehaviour
{
    /////////////////////////////////////////////////   Public properties
    public static bool startupMenuActive {
        get => _inst.startupMenu.activeInHierarchy;
        set {
            _inst.startupMenu.SetActive(value);
        }
    }
    public static bool pauseMenuActive {
        get => _inst.pauseMenu.activeInHierarchy;
        set {
            _inst.pauseMenu.SetActive(value);
            if(!_inst.pauseMenu.activeInHierarchy && !_inst.optionsMenu.activeInHierarchy && !_inst.mainMenu.activeInHierarchy) {
                pauseMenuOpacityGoal = 0f;
                Player.cameraDrawAllowed = true;
            }
            else {
                pauseMenuOpacityGoal = 1f;
                Player.cameraDrawAllowed = false;
            }
        }
    }


    /////////////////////////////////////////////////   Private, Serializable fields
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject vignette;
    [SerializeField] private GameObject redlineVignette;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject item;
    [SerializeField] private GameObject startupMenu;
    [SerializeField] private GameObject background;


    /////////////////////////////////////////////////   Private fields
    private crosshairTypes currentCrosshair;
    private static float pauseMenuOpacityGoal;
    private static Image pauseMenuFill;


   // Singleton instance
    private static UserInterface _inst;
    public static bool isReady { get; private set;}





    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    private void OnEnable()
    {
        _inst = this;
        if(crosshair == null || vignette == null || redlineVignette == null || pauseMenu == null ||
        optionsMenu == null || mainMenu == null || item == null || startupMenu == null) {
            enabled = false;
            throw new System.Exception("All UI child gameObjects are required! Check the 'User Interface' in the inspector");
        }
        background.SetActive(true);
        pauseMenuFill = background.GetComponent<Image>();
        if(pauseMenuFill == null) throw new System.Exception("Pause Menu Background image missing.");
        startupMenuActive = true;
        isReady = true;
    }

    private void Update() {
        pauseMenuFill.fillAmount = Mathf.Lerp(pauseMenuFill.fillAmount, pauseMenuOpacityGoal, Time.fixedUnscaledDeltaTime * 4f);
    }

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
}
