
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


public enum gameStates { menu, paused, normal, redline };
public enum playerStates { clear, mini, drawing, full };
public enum crosshairTypes { draw, yeet, drop, grab, place, nope, none };
public enum QuestState { next, hidden, undiscovered, discovered, completeable, complete };
public enum TravellerType { generic, rockdude, frogwizard, mushroom, wisp };

[Serializable] public struct TravellerPrefab {
    public string name;
    public List<GameObject> Variants;
    public TravellerType type;
}


/// <summary> Controls and state management for the entire game. Classes must inherit from this in order to make changes related to the game's mechanics </summary>
[DisallowMultipleComponent]
public sealed class Expedition : MonoBehaviour
{

    #region [Important]
    /// <summary> All the stuff the Explorer can do. Handles all input. </summary>
    public static S_Player Player { get => S_Player.instance; }
    /// <summary> First-person camera controls. </summary>
    public static S_CameraOperator CameraOperator { get => S_CameraOperator.instance; }
    /// <summary> This class controls WASD movement and jumping. Boing. </summary>
    public static S_Movement Movement { get => S_Movement.instance; }
    /// <summary> Controls user interface elements. </summary>
    public static S_UserInterface UserInterface { get => S_UserInterface.instance; }
    /// <summary> This class handles the player's ability to draw lines. </summary>
    public static S_Drawing Drawing { get => S_Drawing.instance; }
    /// <summary> This class handles the player's ability to use the map. </summary>
    public static S_Map Map { get => S_Map.instance; }

    //public static S_Interaction Interaction { get => S_Interaction.instance; }
    //public static S_Codex Codex { get => S_Codex.instance; }
    //public static S_SaveManager SaveManager { get => S_SaveManager.instance; }
    #endregion

    /////////////////////////////////////////////////   Public properties
    /// <summary> If the game has successfully started or not. </summary>
    public static bool gameplayStarted {get; private set;}

    /// <summary> The layers that are ignored by line drawing raycasts. </summary>
    [SerializeField] private LayerMask _raycastIgnoreLayers;
    public static LayerMask raycastIgnoreLayers { get => _inst._raycastIgnoreLayers; }

    /// <summary>  </summary>
    [SerializeField] private GameObject _RedLineVertexPrefab;
    public static GameObject RedLineVertexPrefab { get => _inst._RedLineVertexPrefab; }

    /// <summary>  </summary>
    [SerializeField] private GameObject _RedLineEdgePrefab;
    public static GameObject RedLineEdgePrefab { get => _inst._RedLineEdgePrefab; }

    /// <summary>  </summary>
    [SerializeField] private GameObject _LineVertexPrefab;
    public static GameObject LineVertexPrefab { get => _inst._LineVertexPrefab; }

    /// <summary>  </summary>
    [SerializeField] private GameObject _LineEdgePrefab;
    public static GameObject LineEdgePrefab { get => _inst._LineEdgePrefab; }

    /// <summary> The renderTexture used by the handheld map's material. Regions cameras can use this as their target texture. </summary>
    [SerializeField] private RenderTexture _MapTexure;
    public static RenderTexture MapTexture { get => _inst._MapTexure; }

    [SerializeField] private Color _questColorUndiscovered;
    public static Color questColorUniscovered { get => _inst._questColorUndiscovered; }

    [SerializeField] private Color _questColorUncompleted;
    public static Color questColorUncompleted { get => _inst._questColorUncompleted; }

    [SerializeField] private Color _questColorCompleted;
    public static Color questColorCompleted { get => _inst._questColorCompleted; }

    [SerializeField] private Color _questColorCompleteable;
    public static Color questColorCompleteable { get => _inst._questColorCompleteable; }

    /// <summary> Initial number of line points samples to project onto surfaces. Will be decimated after. </summary>
    [SerializeField] private int _lineQualityIterations;
    public static int lineQualityIterations { get => _inst._lineQualityIterations;}

    /// <summary> Farthest distance that line projection rays will travel past the line connecting the start and end points. </summary>
    [SerializeField] private float _lineProjectionOvershootDistance;
    public static float lineProjectionOvershootDistance { get => _inst._lineProjectionOvershootDistance;}

    /// <summary> How far out to 'walk' the points from which lines are projected onto the surface between them. </summary>
    [SerializeField] private int _lineProjectionSolverMaxRange;
    public static int lineProjectionSolverMaxRange { get => _inst._lineProjectionSolverMaxRange;}

    /// <summary>  </summary>
    [SerializeField] private int _lineProjectionStartDistance;
    public static int lineProjectionStartDistance { get => _inst._lineProjectionStartDistance;}

    /// <summary>  </summary>
    [SerializeField] private List<TravellerPrefab> _Travellers;
    public static List<TravellerPrefab> Travellers { get => _inst._Travellers; }

    [SerializeField] private float _questCineDuration;
    [SerializeField] private float _questCineScrollFlySmooth; // how fast the scroll flies toward you

    public static LineRenderer IndicatorLine { get; private set; }
    public static AudioSource DrawDroneClear;
    public static AudioSource DrawDroneUnclear;
    public static bool isCinematic { get; private set; }



    /////////////////////////////////////////////////   Private fields
    [SerializeField] private bool autoStartGameWhileEditor;
    private static Camera activeRegionCamera;
    private static S_CameraOperator cam;
    private static GameObject camInitialLookAt;
    private static GameObject camInitialFollowPoint;
    private static GameObject transitionDinghy;
    private static GameObject questScroll;
    private static gameStates _state = gameStates.paused;
    private static bool _paused;
    private static float _timeScaleGoal = 1f;
    private static Vector3 _goalQuestScrollPos;
    private static Vector3 _goalQuestScrollScale;
    private static Quaternion _goalQuestScrollRot;
    private GameObject _hmapTempReference;



    // Singleton instance.
    private static Expedition _inst;







    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    private void Awake() {
        questScroll = transform.Find("Quest Scroll").gameObject;
        _hmapTempReference = GameObject.Find("Hand Map");
        questScroll.SetActive(false);
        IndicatorLine = GetComponentInChildren<LineRenderer>();
        DrawDroneClear = GetComponents<AudioSource>()[0];
        DrawDroneUnclear = GetComponents<AudioSource>()[1];
        DrawDroneClear.volume = 0;
        DrawDroneUnclear.volume = 0;
        _inst = this;
    }

    private void Start()
    {
        startGameplay();

        /*raycastIgnoreLayers = SetRaycastIgnoreLayersHere;
        cam = GameObject.Find("CameraContainer").GetComponent<CameraOperator>(); if (!cam) throw new System.Exception("Camera Container not found. Make sure there is a Camera Container");
        if (SceneManager.GetActiveScene().name == "World")
        {
            canStartup = true;
            camInitialLookAt = cam.lookAt;
            //camInitialFollowPoint = cam.followPoint;
            setState(gameStates.menu);
            transitionDinghy = GameObject.Find("dinghy");
            transitionDinghy.SetActive(false);
        }
        else canStartup = false;
        if (Application.isEditor && autoStartGameWhileEditor) StartGame();
        DontDestroyOnLoad(gameObject);*/
    }

    private void Update() {
        if(isCinematic) {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)) {
                CinematicGetQuestCancel();
            }
            else CinematicGetQuestUpdate();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            SetGamePaused(!_paused);
        }
        Time.timeScale = Mathf.Lerp(Time.timeScale, _timeScaleGoal, Time.fixedUnscaledDeltaTime * 2);
    }



    //////////////////////////////////////////////////////////////////////////////////////////////////  Methods
    private void startGameplay() {
        gameplayStarted = false;
        try {
            if(_inst == null) throw new System.Exception("Expedition.cs singleton instance was SOMEHOW not assigned!");
            if(_RedLineVertexPrefab == null || _RedLineEdgePrefab == null || _LineEdgePrefab == null || _LineVertexPrefab == null || _MapTexure == null) throw new System.Exception("Expedition.cs is missing prefabs. Assign them in the inspector!");

            // Check CameraOperator
            CameraOperator.ObjectToFollow = GameObject.Find("The Explorer");
            if(!CameraOperator.isReady) throw new System.Exception("CameraOperator not ready");
            Debug.Log("<color=green><size=18>Camera Ready</size></color>");

            // Check Movement
            if(!Movement.isReady) throw new System.Exception("Movement not ready");
            Debug.Log("<color=green><size=18>Movement Ready</size></color>");

            // Check Drawing
            if(!Drawing.isReady) throw new System.Exception("Drawing not ready");
            Debug.Log("<color=green><size=18>Drawing Ready</size></color>");

            // Check Mapping
            if(!Map.isReady) throw new System.Exception("Map not ready");
            Debug.Log("<color=green><size=18>Map Ready</size></color>");

            // Check Player
            if(!Player.isReady) throw new System.Exception("Player not ready");
            Debug.Log("<color=green><size=18>Player Ready</size></color>");

            // Check UI
            if(!UserInterface.isReady) throw new System.Exception("UserInterface not ready");
            Debug.Log("<color=green><size=18>UI Ready</size></color>");
            Debug.Log("<color=lime><size=18>Starting Game...</size></color>");

            StartCoroutine(Transition.LoadYourAsyncScene("Overlay"));
        }
        catch(Exception e) {
            Debug.LogException(e);
            Debug.LogError("<color=red><size=18>Could not start game</size></color>");
            return;
        }
        UserInterface.startupMenuActive = false;
        CameraOperator.AllowInput = true;
        Movement.AllowInput = true;
        Player.setPlayerState(playerStates.mini);
        gameplayStarted = true;
    }

    public static void SetGamePaused(bool set) {
        UserInterface.pauseMenuActive = set;
        Movement.AllowInput = !set;
        CameraOperator.AllowInput = !set;
        _timeScaleGoal = (set) ? 0f : 1f;
        _paused = set;
        if(!set) Time.timeScale = 0.9f;
    }

    public static void CinematicGetQuest(Quest quest) {
        _inst.StartCoroutine("IECinematicGetQuest", quest);
    }

    public IEnumerator IECinematicGetQuest(Quest quest) {
        /*MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_BaseMap", quest.QuestIcon);
        questScroll.GetComponent<Renderer>().SetPropertyBlock(block);*/

        var r = questScroll.GetComponent<Renderer>();
        //r.material.EnableKeyword ("_BaseMap");
        r.materials[1].SetTexture("_BaseMap", quest.QuestIcon);

        yield return new WaitForSeconds(0.5f);
        // pre
        Expedition.Player.gameObject.GetComponent<SoundPlayer>().Play("GetQuest");
        Player.setPlayerState(playerStates.mini);
        _hmapTempReference.SetActive(false);
        Movement.AllowInput = false;
        CameraOperator.DoLookAtObject = true;
        CameraOperator.FOV = 55;
        CameraOperator.ObjectToLookAt = quest.proxyScroll.gameObject;
        questScroll.transform.localScale = quest.proxyScroll.localScale;
        questScroll.transform.position = quest.proxyScroll.position;
        questScroll.transform.rotation = quest.proxyScroll.rotation;
        questScroll.SetActive(true);
        questScroll.GetComponent<Animator>().SetBool("IsOpen", false);
        yield return new WaitForSeconds(0.5f);
        isCinematic = true;

        // throw scroll
        yield return new WaitForSeconds(_inst._questCineDuration);

        // unfurl scroll
        Expedition.Player.gameObject.GetComponent<SoundPlayer>().Play("RevealQuest");
        questScroll.GetComponent<Animator>().SetBool("IsOpen", true);
        yield return new WaitForSeconds(2);

        CinematicGetQuestCancel();
    }

    public void CinematicGetQuestUpdate() {
        Transform playerHandheld = GameObject.Find("Quest Scroll Goal").transform;
        //Debug.Log(playerHandheld.position);
        _goalQuestScrollPos = playerHandheld.position;
        _goalQuestScrollRot = playerHandheld.rotation;
        _goalQuestScrollScale = playerHandheld.localScale;
        questScroll.transform.localScale = Vector3.Lerp(questScroll.transform.localScale, _goalQuestScrollScale, Time.deltaTime * _inst._questCineScrollFlySmooth);
        questScroll.transform.position = Vector3.Lerp(questScroll.transform.position, _goalQuestScrollPos, Time.deltaTime * _inst._questCineScrollFlySmooth);
        questScroll.transform.rotation = Quaternion.Slerp(questScroll.transform.rotation, _goalQuestScrollRot, Time.deltaTime * _inst._questCineScrollFlySmooth);
    }

    public void CinematicGetQuestCancel() {
        _inst.StopCoroutine("IECinematicGetQuest");
        Player.setPlayerState(playerStates.mini);
        _hmapTempReference.SetActive(true);
        Movement.AllowInput = true;
        CameraOperator.DoLookAtObject = false;
        CameraOperator.FOV = CameraOperator.defaultFOV;
        CameraOperator.ObjectToLookAt = null;
        questScroll.SetActive(false);
        questScroll.GetComponent<Animator>().SetBool("IsOpen", false);
        isCinematic = false;
    }

    public void LoadBiomeInEditor(string name, bool remove) {
        if(remove) {
            EditorSceneManager.CloseScene(EditorSceneManager.GetSceneByName(name), false);
        }
        else EditorSceneManager.OpenScene("Assets/Scenes/"+name+".unity", OpenSceneMode.Additive);
    }

    /*public static void setState(gameStates state)
    {
        //Debug.Log("Game state changed to "+state);
        Expedition.state = state;

        switch(Expedition.state)
        {
            case gameStates.normal: {
                    //cam.defaultFOV = 90f;
                    //cam.enableControls = true;
                    //cam.enableLookAt = false;
                    //cam.followPoint = GameObject.Find("The Explorer");
                    Camera.main.nearClipPlane = 0.01f;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    //cam.smoothTime = 32;
                    Time.timeScale = 1f;
                    //cam.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 85f, 0));
                    break;
            }
            case gameStates.menu:
            {
                    //cam.defaultFOV = 30f;
                    //cam.enableControls = false;
                    //cam.enableLookAt = true;
                    //cam.lookAt = camInitialLookAt;
                    //cam.followPoint = camInitialFollowPoint;
                    Camera.main.GetComponent<Animator>().enabled = true;
                    Camera.main.nearClipPlane = 1;
                    //cam.smoothTime = 2;
                    S_UserInterface.SetCursor(crosshairTypes.none);
                    break;
            }
            case gameStates.paused:
            {
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    //cam.enableControls = false;
                    break;
            }
            case gameStates.redline:
            {
                    Time.timeScale = 0.5f;
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    //cam.enableControls = false;
                    break;
            }
        }
    }*/



    /*public static Region getActiveRegion()
    {
        return activeRegion;
    }

    public static void setActiveRegion(Region region)
    {
        activeRegion = region;
        if(region != null)
        {
            //activeRegion.setRenderTexture();
        }
    }

    public static Portal getPortalOfActiveRegion()
    {
        var r = getActiveRegion();
        if (r != null) return r.getActivePortal();
        else return null;
    }*/

    /*public static gameStates getState()
    {
        return state;
    }

    private static IEnumerator checkIfGameIsReady(float delay) {
        yield return new WaitForSeconds(delay);
    }

    private static IEnumerator flyToStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        setState(gameStates.normal);
        Destroy(GameObject.Find("HideAfterTransition"));
        transitionDinghy.SetActive(true);
        Camera.main.GetComponent<Animator>().enabled = false;
        //inst.StartCoroutine(camr(0.1f));
        
    }

    private static IEnumerator camr(float delay)
    {
        yield return new WaitForSeconds(delay);
        cam.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 85f, 0));
    }

    public static void StartGame()
    {
        //if (!canStartup) return;
        GameObject mm = GameObject.Find("MainMenu");
        if (mm != null) mm.SetActive(false);
		GameObject bo = GameObject.Find("S_Boat");
        GameObject ex = GameObject.Find("The Explorer");
        GameObject.Find("Logo").SetActive(false);
        //cam.followPoint = ex;
        //cam.defaultFOV = 90f;
        //cam.lookAt = bo;
        //cam.smoothTime = 0.75f;
		ex.transform.LookAt(bo.transform);
        CameraOperator.ObjectToFollow = null;
		//inst.StartCoroutine(flyToStart(Application.isEditor ? 0.1f : 5f));
    }*/

    /*public static GameObject getTraveller()
    {
        return GameObject.Findb;
    }*/

    /*public static void startTraveller()
    {
        var trav = Expedition.getTraveller().GetComponent<NavMeshMovement>();
        trav.spawn(activePortal.transform.position);
        trav.givePath(activeRegion.getLatestRedLine());
        trav.navMove();
    }*/
}

// Menu: In the main menu of the game.
// Paused: Game is paused.
// Normal: Explorer can move around and do stuff.
// Fullmap: We are in fullscreen map mode; no movement allowed.
// Guiding: TODO not sure about this one.

/*using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum BiomeTypes { None, Tutorial, Foggy, Mountain, Desert, Caves, };
public class Region : MonoBehaviour
{
    public BiomeTypes biome = BiomeTypes.None;
    public string name = "Region";
    public bool completed;
    public int lineLimit = 25;
    public RenderTexture textureTarget;
    public bool isStartingRegion;

    //private List<Vector3> redLine = new List<Vector3>();
    private GameObject background;
    private Camera cam;
    public GameObject templateLine;
    public GameObject templateRedLine;
    private List<GameObject> lines =  new List<GameObject>();
    private List<GameObject> redLines = new List<GameObject>();
    private List<GameObject> redoLines = new List<GameObject>();
    private List<Portal> portals = new List<Portal>();

    void Awake()
    {
        if (transform.childCount != 2) throw new System.Exception("Region '" + name + "' has an invalid number of children!");
        background = transform.Find("Map Background").gameObject;
        if(!background) throw new System.Exception("Region '" + name + "' does not have a background!");
        cam = GetComponentInChildren<Camera>();
        if (!cam) throw new System.Exception("Region '" + name + "' does not have a camera!");
        //templateLine = GameObject.Find("Line");
        //templateRedLine = GameObject.Find("RedLine");
        if (!templateLine) throw new System.Exception("Region does not have 'Line' prefab!");
        if (!templateRedLine) throw new System.Exception("Region does not have 'Redline' prefab!");

        // Check for overlapping map regions.
        if (Application.isEditor)
        {
            // https://docs.unity3d.com/ScriptReference/Physics.OverlapBox.html
            //Use the OverlapBox to detect if there are any other colliders within this box area.
            //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
            Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
            int i = 0;
            //Check when there is a new collider coming into contact with the box
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].tag == tag && hitColliders[i].gameObject != gameObject)
                {
                    // https://gamedev.stackexchange.com/questions/172151/how-to-change-material-color-lwrp
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    block.SetColor("_BaseColor", Color.red);
                    GetComponent<Renderer>().SetPropertyBlock(block);
                    hitColliders[i].gameObject.GetComponent<Renderer>().SetPropertyBlock(block);
                    throw new System.Exception("Intersecting regions!");
                }
                if(hitColliders[i].tag == "Portal" && hitColliders[i].gameObject != gameObject)
                {
                    Debug.Log("region '"+name+"' found an intersecting portal");
                    var p = hitColliders[i].transform.gameObject.GetComponent<Portal>();
                    p.addOwnerRegion(this);
                    portals.Add(p);
                }
                i++;
            }
        }
    }

    private void Start()
    {
        if (isStartingRegion) activateCamera(true);
    }

    // Set the active region upon entering.
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            StateController.activeRegion = this;
            StateController.activeRegionCamera = GetComponentInChildren<Camera>();
            activateCamera(true);
            //Debug.Log("Active region: " + StateController.activeRegion.name);
            foreach (GameObject l in lines) {l.SetActive(true);} // Show lines.
            GetComponent<SoundPlayer>().Play("Enter");
        }
    }

    // Set active region to null on exit. If the player just walked into a new region, it'll become active a moment after this.
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().endCameraLine(false);
            activateCamera(false);
            StateController.activeRegion = null;
            StateController.activeRegionCamera = null;
            foreach (GameObject l in lines) {l.SetActive(false);} // Hide lines.
        }
    }

    private IEnumerator checkIfEmptyRegion(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!StateController.activeRegion)
        {
            //Debug.Log("Active region: none");
        }
    }






    // Instantiate a new Line using the template Line.
    public void addLineToRegion(Vector3 pos)
    {
        if (transform.childCount - 2 >= lineLimit) {
            Debug.LogWarning("Cannot add line to region '" + name + "', limit reached!");
            return;
        }
        GameObject createdLine = Instantiate(templateLine, pos, Quaternion.identity, transform);
        redoLines = new List<GameObject>(); // Gets rid of redo lines.
        lines.Add(createdLine);

    }

    // Add a new point to this region's latest Line.
    public void addLinePointToRegion(Vector3 pos)
    {
        if (lines.Count < 1) {
            Debug.LogWarning("Cannot add points; this region has no active line!");
            return;
        }
        lines[lines.Count - 1].GetComponent<Line>().addPoint(pos);
    }



    //////////////////////////////////////////////////////////////////////////
    // Instantiate a new Line using the template red Line.
    public void addRedLineToRegion(Vector3 pos)
    {
        foreach (GameObject x in redLines) {
            Destroy(x);
        }
        redLines = new List<GameObject>();
        if (pos == Vector3.zero)
        {
            Debug.LogWarning("Cannot add RED line, vector zero recieved!");
            return;
        }
        if (transform.childCount - 2 >= lineLimit)
        {
            Debug.LogWarning("Cannot add RED line to region '" + name + "', limit reached!");
            return;
        }
        GameObject createdLine = Instantiate(templateRedLine, pos, Quaternion.identity, transform);
        redLines.Add(createdLine);

    }

    // Add a new point to this region's latest red Line.
    public void addRedLinePointToRegion(Vector3 pos)
    {
        if (pos == Vector3.zero)
        {
            Debug.LogWarning("Cannot add RED line, vector zero recieved!");
            return;
        }
        if (redLines.Count < 1)
        {
            Debug.LogWarning("Cannot add RED line points; this region has no active line!");
            return;
        }
        redLines[redLines.Count - 1].GetComponent<Line>().addPoint(pos);
    }




    //////////////////////////////////////////////////////////////////////////
    // Set this Region's camera to use the renderTexture used by the handheld map material.
    private void activateCamera(bool tf)
    {
        cam.targetTexture = tf ? textureTarget : null;
    }

    // Sink the latest Line.
    public void sinkLatestLine()
    {
        if(lines.Count > 0)
        {
            lines[lines.Count - 1].GetComponent<Line>().sinkLine(true);
        }
        else Debug.Log("Cannot sink latest line, no lines to sink");
    }




    //////////////////////////////////////////////////////////////////////////
    // Removes a Line and adds it to the redo list.
    public void undoLine()
    {
        if (lines.Count == 0)
        {
            Debug.LogWarning("Cannot undo, no lines left!");
            return;
        }
        redoLines.Add(lines[lines.Count - 1]);
        lines[lines.Count - 1].SetActive(false);
        lines.RemoveAt(lines.Count - 1);
    }

    // Removes a Line from the redo list and adds it to the normal list.
    public void redoLine()
    {
        if (redoLines.Count == 0)
        {
            Debug.LogWarning("Nothing to redo.");
            return;
        }
        redoLines[redoLines.Count - 1].SetActive(true);
        lines.Add(redoLines[redoLines.Count - 1]);
        redoLines.RemoveAt(redoLines.Count - 1);
    }

    // Check if all this region's portals are finished.
    public bool checkForCompletion()
    {
        foreach(Portal x in portals)
        {
            if (!x.isComplete())
            {
                completed = false;
                return false;
            }
        }
        Debug.Log("REGION COMPLETE!");
        completed = true;
        return true;
    }

    public List<Vector3> getLatestRedLine()
    {
        if (redLines.Count == 0) Debug.LogWarning("Can't get redline, region has no redlines.");
        var r = redLines[redLines.Count - 1].GetComponent<Line>().getPoints();
        
        return r;
    }
}*/

