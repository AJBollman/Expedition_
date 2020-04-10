
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum gameStates { menu, paused, normal, redline };
public enum playerStates { clear, mini, drawing, full };
public enum crosshairTypes { draw, yeet, drop, grab, place, nope, none };
public enum QuestState { next, hidden, undiscovered, discovered, completeable, complete };
public enum TravellerType { generic, rockdude, frogwizard, mushroom, wisp };
public enum BiomeScene { overlay, world, plains, glowcaves, foggyforest, crystalcaves, shipwreck, mountain, credits }

[Serializable] public struct TravellerPrefab {
    public string name;
    public List<GameObject> Variants;
    public TravellerType type;
}
[Serializable] public struct BiomeData {
    public BiomeScene scene;
    public AudioClip music;
    public Color32 sunColor;
    public GameObject boundingBox;
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
    /// <summary> </summary>
    public static S_Atmosphere Atmosphere { get => S_Atmosphere.instance; }

    //public static S_Interaction Interaction { get => S_Interaction.instance; }
    //public static S_Codex Codex { get => S_Codex.instance; }
    //public static S_SaveManager SaveManager { get => S_SaveManager.instance; }
    #endregion

    #region [Public]
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
    /*
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
    */
    /// <summary>  </summary>
    [SerializeField] private List<TravellerPrefab> _Travellers;
    public static List<TravellerPrefab> Travellers { get => _inst._Travellers; }

    /// <summary> Travellers's line-of-sight raycast will be raised by this height. </summary>
    [SerializeField] private float _travellerLOSHeight;
    public static float travellerLOSHeight { get => _inst._travellerLOSHeight; }

    /// <summary> Layers that a traveller can see through during LOS check. </summary>
    [SerializeField] private LayerMask _travellerLOSMask;
    public static LayerMask travellerLOSMask { get => _inst._travellerLOSMask; }

    /// <summary>  </summary>
    [SerializeField] private List<BiomeData> _Biomes;
    public static List<BiomeData> Biomes { get => _inst._Biomes; }

    /// <summary> Return info on the biome the Explorer is currently in. </summary>
    public static BiomeData activeBiome => _inst._Biomes[activeBiomeIndex];

    [SerializeField] private float _questCineDuration;
    [SerializeField] private float _questCineScrollFlySmooth; // how fast the scroll flies toward you
    [SerializeField] private float _smoothMusicTransition;

    /// <summary> Sunlight color transition speed. </summary>
    public static float smoothLightTransition => _inst._smoothLightTransition;
    [SerializeField] private float _smoothLightTransition;
    [SerializeField] private float _musicVolume;

    public static LineRenderer IndicatorLine { get; private set; }
    public static AudioSource DrawDroneClear;
    public static AudioSource DrawDroneUnclear;
    public static bool isCinematic { get; private set; }
    #endregion


    #region [Private]
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
    private static int activeBiomeIndex;
    private static AudioSource MusicSource;
    private static bool _musicIsMuted;
    private static AudioClip _QueuedClip;
    #endregion




    #region [Events]
    private void Awake() {
        questScroll = transform.Find("Quest Scroll").gameObject;
        _hmapTempReference = GameObject.Find("Hand Map");
        questScroll.SetActive(false);
        IndicatorLine = GetComponentInChildren<LineRenderer>();
        DrawDroneClear = GetComponents<AudioSource>()[0];
        DrawDroneUnclear = GetComponents<AudioSource>()[1];
        MusicSource = GetComponents<AudioSource>()[2];
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

        MusicSource.volume = Mathf.Lerp(MusicSource.volume, (_musicIsMuted) ? 0 : _musicVolume, Time.deltaTime * _smoothMusicTransition);
    }
    #endregion


    #region [Methods]
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

            // Check Atmosphere
            if(!Atmosphere.isReady) throw new System.Exception("Atmosphere not ready");
            Debug.Log("<color=green><size=18>Atmosphere Ready</size></color>");
            Debug.Log("<color=lime><size=18>Starting Game...</size></color>");
        }
        catch(Exception e) {
            Debug.LogException(e);
            Debug.LogError("<color=red><size=18>Could not start game</size></color>");
            return;
        }
        try {
            // 'Spawn' the explorer at the first 'spawn' prefab found.
            AutoInitializer[] spawns = GameObject.FindObjectsOfType<AutoInitializer>();
            if(spawns.Length > 0) Player.gameObject.transform.position = spawns[0].transform.position + (Vector3.up);
            else Debug.Log("No spawns found");
        }
        catch(Exception e2) {Debug.LogException(e2);}
        UserInterface.startupMenuActive = false;
        CameraOperator.AllowInput = true;
        Movement.AllowInput = true;
        Player.setPlayerState(playerStates.mini);
        BiomeTransition(1);
        SetMusicMute(false, true);
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

    public static void CheckGameCompletion() {
        int completeQuestCount = 0;
        foreach(Quest q in Quest.AllQuests) {
            if(q.state == QuestState.complete) completeQuestCount++;
        }
        float check = completeQuestCount / Quest.AllQuests.Count;
        if(check == 1) {
            Debug.Log("<color=lime><size=18>GAME COMPLETE!!</size></color>");
        }
        else {
            Debug.Log("<color=cyan><size=18>"+ check * 100f +" % complete</size></color>");
        }
    }

    public static void BiomeTransition(int sceneIndex) {
        if(sceneIndex < 1 || sceneIndex > Biomes.Count-1) throw new Exception("Transition index out of bounds");
        activeBiomeIndex = sceneIndex;
        Atmosphere.GoalSunColor = Biomes[sceneIndex].sunColor;
        _QueuedClip = Biomes[sceneIndex].music;
    }
    
    public static void SetMusicMute(bool mute, bool isTransition) {
        _musicIsMuted = mute;
        if(!_musicIsMuted && isTransition) {
            MusicSource.clip = _QueuedClip;
            MusicSource.Play();
        }
    }
    #endregion

    private static Expedition _inst;
}