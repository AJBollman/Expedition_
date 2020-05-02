
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary> Dispatches function calls to other classes based on player input. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(S_Movement))]
[RequireComponent(typeof(SoundPlayer))]
public sealed class S_Player : MonoBehaviour
{
    /// <summary> The Explorer gameobject. </summary>
    public static GameObject Explorer { get; private set; }

    [SerializeField] private bool drawDebugLines;
    public RaycastHit lastRaycastHit { get; private set; }
    public playerStates PlayerState { get => _playerState; }
    // What distance from the cursor should be considered valid for completing a quest's redline.
    [SerializeField] private float validRedLineDistance;
    private SoundPlayer _Sound;
    private Vector3 lastEditorTP;
    private playerStates _playerState;



    #region preRework
    public static bool cameraDrawAllowed = true;
    public string cameraDrawButton = "Fire1";
    public float cameraDrawMaxDistance = 15;
    public LayerMask raycastIgnoreLayers;
    public float coordPlaneSizeFudgeFactor = 1f; //0.82821

    public bool undoRedoAllowed = true;
    public string undoKey = "z";
    public string redoKey = "x";
    public bool fullMapAllowed = true;
    public string fullKey = "f";
    public float mapFullscreenTransitionTime = 5f;
    public Vector3 fullMapPos = new Vector3(0, 0, 0.12f);
    public Vector3 miniMapPos = new Vector3(0.26f, -0.1f, 0.23f);
    public string toggleMapRotKey = "l";
    public bool redLineAllowed = true;
    public string redLinekey = "r";
    public bool fullscreenLineDrawAllowed = false;
    //public LayerMask droppedItemCheck;
    public float recallPlankIfLowerThan = 12f;

    public static bool mapIsFull;
    public static bool mapSpins;
    public static bool isRedLineMode;

    public bool isCameraDrawing;
    private GameObject vignette;
    private GameObject redvignette;
    private GameObject handMap;
    private S_CameraOperator cam;
    private GameObject coordOrigin;
    private GameObject measure;
    private Vector2 coordPlaneBounds;
    private CharacterController ccon;
    private HoldItems holdItems;
    private GameObject PauseMenu;

    private S_Movement mv;
    private float preFOV;
    private float preFOVT;
    private bool undoRedoPre;
    private bool inDrawingRange;
    private gameStates cachedGS;

    private AudioSource jumpLoopSrc;
    public AudioClip jumpLoop;
    private AudioSource drawLoopSrc;
    public AudioClip drawLoop;
    private static SoundPlayer sound;
    #endregion



    //////////////////////////////////////////////////////////////////////////////////////////////////
    #region [Events]
    private void OnEnable() {
        instance = this;
        try {
            Explorer = gameObject;
            _Sound = GetComponent<SoundPlayer>();
            coordOrigin = GameObject.Find("Lower Left Corner"); if (!coordOrigin) throw new System.Exception("Hand Map coordinate origin not found. Make sure there is a 'coordOrigin'");
            measure = GameObject.Find("Upper Right Corner"); if (!measure) throw new System.Exception("Measure not found. Make sure there is a 'Measure'");
            coordPlaneBounds = new Vector2(
                Vector3.Distance(coordOrigin.transform.position, measure.transform.position),
                Vector3.Distance(coordOrigin.transform.position, measure.transform.position)
            );

            isReady = true;
        }
        catch(Exception e) {
            isReady = false;
            enabled = false;
            Debug.LogException(e);
        }
    }

    private IEnumerator soundd() {
        yield return new WaitForSeconds(0.08f);
        Expedition.Drawing.connectVertex();
        cameraDrawAllowed = true;
        Expedition.Drawing._Indicator.transform.localScale = Vector3.one;
    }

    void Update()
    {
        // undo redo
        if(Input.GetKeyDown(KeyCode.Z)) {
            Expedition.Drawing.Undo();
        }
        if(Input.GetKeyDown(KeyCode.X)) {
            Expedition.Drawing.Redo();
        }

        #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P) && Application.isEditor) {
            EditorApplication.isPaused = true;
        }
        #endif

        // if we are in fullmap mode while inside a completeable quest,
        Vector2 mapPos = Vector2.zero;
        Vector3 globalPos =  Vector3.zero;
        bool mapWasHit = false;
        bool questSolutionIsValid = false;
        if(_playerState == playerStates.full && Quest.Active != null && Quest.Active.state == QuestState.completeable) {
            // check if the mouse cursor is on a valid spot on the map.
            mapPos = mousePosToMapPos();
            globalPos = Expedition.Map.MapToWorldPos(mapPos);
            if(mapPos != Vector2.zero){
                mapWasHit = true;
                Expedition.Map.MoveLatestVertex(globalPos);
                questSolutionIsValid = Vector3.Distance( globalPos, Quest.Active.EndPoint.transform.position ) < validRedLineDistance;
                if(questSolutionIsValid) {
                    // particle effect
                }
            }
        }

        // mouse down.
        if (Input.GetButtonDown(cameraDrawButton) && cameraDrawAllowed) {
            if(_playerState == playerStates.drawing && Expedition.Drawing.hasLOS) {
                _Sound.Play("PlaceVertex");
                Expedition.Drawing.placeVertex();
                Expedition.Drawing._Indicator.transform.localScale = new Vector3(0.5f, 1, 0.5f);
            }
            else if(mapWasHit) {
                Expedition.Map.placeRedVertex(globalPos);
                if(questSolutionIsValid) {
                    Expedition.Map.SolveQuest();
                }
                else {
                    
                }
            }
        }

        // mouse up.
        if (Input.GetButtonUp(cameraDrawButton) && cameraDrawAllowed && _playerState == playerStates.drawing)
        {
            if(Expedition.Drawing.hasLOS) {
                cameraDrawAllowed = false;
                _Sound.Play("ConnectVertex");
                StartCoroutine("soundd");
            }
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            var state = ((int)_playerState + 1);
            if(state > 3) state = 2;
            setPlayerState( (playerStates)state );
        }

        if(Input.GetKeyDown(KeyCode.Q)) {
            var state = ((int)_playerState - 1);
            if(state < 0) state = 0;
            setPlayerState( (playerStates)state );
        }

    } // end update
    #endregion



    #region [Methods]
    public void setPlayerState(playerStates state) {
        if(Expedition.isCinematic) return;
        _playerState = state;
        Expedition.Drawing.attemptToHideLastLine();
        if(Expedition.Drawing.IsDrawing) {
            Expedition.Drawing.cancelLine();
            _Sound.Play("DrawFail");
        }
        Expedition.Map.CancelRedLine();
        Expedition.DrawDroneClear.volume = 0;
        Expedition.DrawDroneUnclear.volume = 0;
        Expedition.IndicatorLine.positionCount = 0;
        switch(_playerState) {
            case playerStates.clear: {
                Expedition.CameraOperator.AllowInput = true;
                Expedition.Movement.AllowInput = true;
                Expedition.Drawing.AllowCameraDrawing = false;
                Expedition.Map.IsMapVisible = false;
                Expedition.Map.IsFullMap = false;
                Expedition.Map.HideMapSpecial();
                break;
            }
            case playerStates.mini: {
                Expedition.CameraOperator.AllowInput = true;
                Expedition.Movement.AllowInput = true;
                Expedition.Drawing.AllowCameraDrawing = false;
                Expedition.Map.IsMapVisible = true;
                Expedition.Map.IsFullMap = false;
                Expedition.Map.ShowMapSpecial();
                break;
            }
            case playerStates.drawing: {
                Expedition.CameraOperator.AllowInput = true;
                Expedition.Movement.AllowInput = true;
                Expedition.Drawing.AllowCameraDrawing = true;
                Expedition.Map.IsMapVisible = true;
                Expedition.Map.IsFullMap = false;
                Expedition.Map.ShowMapSpecial();
                break;
            }
            case playerStates.full: {
                Expedition.CameraOperator.AllowInput = false;
                Expedition.Movement.AllowInput = false;
                Expedition.Drawing.AllowCameraDrawing = false;
                Expedition.Map.IsMapVisible = true;
                Expedition.Map.IsFullMap = true;
                Expedition.Map.ShowMapSpecial();
                if(Quest.Active != null && Quest.Active.state == QuestState.completeable) {
                    Expedition.Map.centerCameraOnQuest(Quest.Active);
                    Expedition.Map.StartNewRedline();
                }
                break;
            }
        }
    }

    public bool viewRaycast(float range, Vector2 pos) {
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(pos.x, pos.y, 0f)), out RaycastHit hit, range, layerMask: Expedition.raycastIgnoreLayers)) {
            // Optionally, show the raycast in scene view.
            if(drawDebugLines) Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            lastRaycastHit = hit;
            return true;
        }
        else return false;
    }

    public Vector2 mousePosToMapPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 10))
        {
            if (hit.collider.gameObject.name == "Hand Map")
            {
                // coordinate geometry horribleness.
                var correctedBounds = coordPlaneBounds.y * coordPlaneSizeFudgeFactor;
                Vector2 pos = new Vector2(
                    (Mathf.Abs(coordOrigin.transform.InverseTransformPoint(hit.point).x - coordOrigin.transform.localPosition.x) / correctedBounds) / 100f,
                    (Mathf.Abs(coordOrigin.transform.localPosition.y - coordOrigin.transform.InverseTransformPoint(hit.point).y) / correctedBounds) / 100f
                );
                return pos;
            }
            // Else the mouse is off the map.
        }
        // Else the ray didn't hit anything.
        return Vector2.zero;
    }

    #endregion



    public static S_Player instance { get; private set; }
    public bool isReady { get; private set;}

}