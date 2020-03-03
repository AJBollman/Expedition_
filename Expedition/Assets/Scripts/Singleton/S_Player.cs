
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
            /*sound = GetComponent<SoundPlayer>();
            jumpLoopSrc = gameObject.AddComponent<AudioSource>();
            jumpLoopSrc.clip = jumpLoop;
            jumpLoopSrc.loop = true;
            drawLoopSrc = gameObject.AddComponent<AudioSource>();
            drawLoopSrc.clip = drawLoop;
            drawLoopSrc.loop = true;*/

            isReady = true;
        }
        catch(Exception e) {
            isReady = false;
            enabled = false;
            Debug.LogException(e);
        }
    }

    private void Start()
    {
        /*DontDestroyOnLoad(gameObject);
        ccon = GetComponent<CharacterController>();
        vignette = GameObject.Find("Vignette"); if (!vignette) throw new System.Exception("Vignette not found. Make sure there is a gameObject named 'Vignette'");
        redvignette = GameObject.Find("Redline Vignette"); if (!redvignette) throw new System.Exception("Redline Vignette not found. Make sure there is a gameObject named 'Redline Vignette'");
        cam = GameObject.Find("CameraContainer").GetComponent<CameraOperator>(); if (!cam) throw new System.Exception("Camera Container not found. Make sure there is a Camera Container");
        handMap = GameObject.Find("HandMap Offset"); if (!handMap) throw new System.Exception("Hand Map Offset not found. Make sure there is a 'Hand Map Offset'");
        coordOrigin = GameObject.Find("CoordOrigin"); if (!coordOrigin) throw new System.Exception("Hand Map coordinate origin not found. Make sure there is a 'coordOrigin'");
        measure = GameObject.Find("Measure"); if (!measure) throw new System.Exception("Measure not found. Make sure there is a 'Measure'");
        holdItems = GetComponent<HoldItems>();
        holdItems.guide = Camera.main.gameObject.transform.GetChild(1).gameObject.transform;
        PauseMenu = GameObject.Find("PauseMenu");

        preFOV = 90f;
        preFOVT = cam.maxFOVTweak;
        mv = GetComponent<Movement>();
        // COULD SUPPORT NONSQUARE MAP IN THE FUTURE BUT NOT NOW V  V  V
        coordPlaneBounds = new Vector2(
            Vector3.Distance(coordOrigin.transform.position, measure.transform.position),
            Vector3.Distance(coordOrigin.transform.position, measure.transform.position)
        );
        Debug.Log("CoordPlaneBounds is " + coordPlaneBounds.x + ". Sound right?");

        jumpLoopSrc = gameObject.AddComponent<AudioSource>();
        jumpLoopSrc.clip = jumpLoop;
        jumpLoopSrc.loop = true;
        drawLoopSrc = gameObject.AddComponent<AudioSource>();
        drawLoopSrc.clip = drawLoop;
        drawLoopSrc.loop = true;
        PauseMenu.SetActive(false);
        redvignette.SetActive(false);
        vignette.SetActive(false);*/

        
    }

    private IEnumerator soundd() {
        yield return new WaitForSeconds(0.08f);
        Expedition.Drawing.connectVertex();
        cameraDrawAllowed = true;
        Expedition.Drawing._Indicator.transform.localScale = Vector3.one;
    }

    void Update()
    {
        // debug teleport.
        if(Input.GetKeyDown(KeyCode.Z) && Application.isEditor) {
            if( viewRaycast(Mathf.Infinity, new Vector2(0.5f, 0.5f)) ) {
                lastEditorTP = transform.position;
                transform.position = lastRaycastHit.point + new Vector3(0, 1f, 0);
            }
        }
        if(Input.GetKeyDown(KeyCode.X) && Application.isEditor && lastEditorTP != Vector3.zero) {
            transform.position = lastEditorTP;
        }
        if(Input.GetKeyDown(KeyCode.P) && Application.isEditor) {
            EditorApplication.isPaused = true;
        }

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
                Vector3 levelledQuestEndpointPos = new Vector3(Quest.Active.EndPoint.transform.position.x, globalPos.y, Quest.Active.EndPoint.transform.position.z);
                questSolutionIsValid = Vector3.Distance( globalPos, levelledQuestEndpointPos ) < validRedLineDistance;
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
        if(Expedition.Drawing.IsDrawing) {
            Expedition.Drawing.cancelLine();
            _Sound.Play("DrawFail");
        }
        Expedition.Map.CancelRedLine();
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

    /*private void startCameraLine()
    {
        if (!cameraDrawAllowed || !Expedition.getActiveRegion()) return;

        var r = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance, layerMask: raycastIgnoreLayers))
        {
            inDrawingRange = true;
            //Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            Vector3 newHit = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            lastRaycastHit = newHit;
            Expedition.getActiveRegion().addLine(newHit);
            GetComponent<SoundPlayer>().Play("DrawStart");
        }
        else inDrawingRange = false;
    }
    // Add a point to the active Map Line under the Active Region.
    private void addToCameraLine()
    {
        if (!cameraDrawAllowed || !Expedition.getActiveRegion() || !isCameraDrawing) return;

        var r = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance, layerMask: raycastIgnoreLayers))
        {
            inDrawingRange = true;
            //Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            Vector3 newHit = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            lastRaycastHit = newHit;
            Expedition.getActiveRegion().addLinePoint(newHit);
            if (!drawLoopSrc.isPlaying) drawLoopSrc.Play();
        }
        else inDrawingRange = false;
    }
    private void FixedUpdate()
    {
        return;
        var mag = Mathf.Abs(Mathf.Max(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))) * 0.75f + (ccon.velocity.magnitude * 0.1f);
        drawLoopSrc.volume = mag;
        cachedGS = Expedition.getState();
    }




    /////////////////////////////////////////////////////////   UNDO AND REDO LINES
    // Undoes the last drawn Line in the active Region.
    private void undoLastLine()
    {
        if (!undoRedoAllowed) return;
        Expedition.getActiveRegion().undoLine();
    }
    // Redoes the last drawn Line in the active Region.
    private void redoLastLine()
    {
        if (!undoRedoAllowed) return;
        Expedition.getActiveRegion().redoLine();
    }





    /////////////////////////////////////////////////////////   FULLSCREEN MAP
    public void toggleFullMap()
    {
        if (Expedition.getActiveRegion() == null) return;
        mapIsFull = !mapIsFull;
        if (mapIsFull && Expedition.getPortalOfActiveRegion() != null) isRedLineMode = true;
        //cam.maxFOVTweak = (mapIsFull) ? 0f : preFOVT;
        cameraDrawAllowed = !mapIsFull;
        undoRedoAllowed = !mapIsFull;
        //mv.moveAllowed = !mapIsFull;
        if (mapIsFull)
        {
            GetComponent<SoundPlayer>().Play("MapFull");
        }
        else
        {
            GetComponent<SoundPlayer>().Play("MapMinimize");
        }
    }





    /////////////////////////////////////////////////////////   TOGGLE MAP SPINNING
    ///// todo make this an option in a future options menu.
    public void toggleMapRot()
    {
        mapSpins = !mapSpins;

    }





    /////////////////////////////////////////////////////////   DRAWING ON MAP
    // Get the position of the mouse relative to the handheld map. Returns zero by default.
    public Vector2 mousePosToMapPos()
    {
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.collider.gameObject.name == "HandMap")
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

    // Get world position by raycasting from the active region camera (if the player is in a region).
    public Vector3 mapPosToWorldPos(Vector2 mapPos) {
        if(!Expedition.getActiveRegion()) {
            throw new Exception("Can't raycast from region camera, no active region!");
        }
        return Expedition.getActiveRegion().raycastFromRegionCamera(mapPos);
    }





    /////////////////////////////////////////////////////////   DRAWING RED LINE
    // Instantiate a new Red Line prefab under active Region.
    private void startRedLine()
    {
        if (Expedition.getPortalOfActiveRegion() == null)
        {
            Debug.LogWarning("Can't start redline, not in a portal!");
            return;
        }
        Vector2 mapPos = mousePosToMapPos(); // where the mouse cursor is
        Vector3 worldPos = mapPosToWorldPos(mapPos); // where the point on the map is, in world space
        Expedition.getActiveRegion().restartRedLine(worldPos);
    }

    // Add a point to the active Red Line under the Active Region.
    private void addToRedLine()
    {
        if (Expedition.getPortalOfActiveRegion() == null) {
            throw new Exception("Can't add point to the redline, not in a portal!");
        }
        Vector2 mapPos = mousePosToMapPos(); // get mouse pos on map.
        Vector3 worldPos = mapPosToWorldPos(mapPos);
        Expedition.getActiveRegion().addRedLinePoint(worldPos);
    }

    // Start the traveller from the current portal.
    private void endRedLine()
    {
        if(Expedition.getPortalOfActiveRegion() == null) {
            throw new Exception("Can't end the redline, not in a portal!");
        }
        //Expedition.getPortalOfActiveRegion().startTraveller();
        Debug.Log("START TRAVELLER HERE");
        toggleFullMap(); // put the map down.
    }




    /////////////////////////////////////////////////////////   OTHER STUFF
    // Set crosshair to indicate how you can interact with things.
    private void checkInteract()
    {
        if (Expedition.getState() != gameStates.normal) return;
        if (isCameraDrawing)
        {
            if (inDrawingRange && Expedition.getActiveRegion() != null)
            { // can draw a line.
                UserInterface.SetCursor(crosshairTypes.draw);
            }
            else // trying to draw a line but cant.
            {
                UserInterface.SetCursor(crosshairTypes.none);
            }
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)));
        RaycastHit hit;
        var hol = holdItems.getCheckOnDropped();
        if (hol != null && Physics.Raycast(ray, out hit, 100f, layerMask: droppedItemCheck) && hit.transform.gameObject != null && hit.transform.gameObject == hol)
        {
                UserInterface.SetCursor(crosshairTypes.grab);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    holdItems.Pickup(hit.transform.gameObject);
                }
            return;
        }
        if (hol != null && transform.position.y - hol.transform.position.y > recallPlankIfLowerThan)
        {
            holdItems.Pickup(hol);
        }

        if (Physics.Raycast(ray, out hit, 4f))
        {
            if (hit.transform.gameObject != null)
            {
                if (hit.transform.gameObject.tag == "Moveable") // moveable object, can grab it.
                {
                    UserInterface.SetCursor(crosshairTypes.grab);
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (holdItems.getHeldObject() != null)
                        {
                            holdItems.Drop(false);
                        }
                        else
                        {
                            holdItems.Pickup(hit.transform.gameObject);
                        }
                    }
                }
                else if (holdItems.getHeldObject() != null)
                {
                    if (hit.transform.gameObject.tag == "Event")
                    // Looking at an event.
                    {
                        if (hit.transform.gameObject.name.Contains(holdItems.getHeldObject().name))
                        // looking at an event while holding the proper item.
                        {
                            UserInterface.SetCursor(crosshairTypes.place);
                            if (Input.GetKeyDown(KeyCode.E))
                            {
                                var test = hit.transform.gameObject.GetComponent<BasicEvent>();
                                if (test != null)
                                {
                                    test.completeEvent(holdItems.getHeldObject());
                                    holdItems.Place(hit.transform.gameObject.transform);
                                }
                                else throw new System.Exception("This event object has no BasicEvent script!");
                            }
                        }
                        else
                        { // looking at an event while holding the wrong item.
                            UserInterface.SetCursor(crosshairTypes.nope);
                        }
                    }
                    else // cant place our item, but we are still holding an item, so it can be dropped.
                    {
                        UserInterface.SetCursor(crosshairTypes.drop);
                        if (Input.GetKeyDown(KeyCode.E)) holdItems.Drop(false);
                    }
                }
                else // looking at the ground, but nothing to do.
                {
                    UserInterface.SetCursor(crosshairTypes.none);
                }
            }
        }
        else // not in interact range.
        {
            if (holdItems.getHeldObject() != null) // if holding something, can yeet it.
            {
                UserInterface.SetCursor(crosshairTypes.yeet);
                if (Input.GetKeyDown(KeyCode.E)) holdItems.Drop(true);
            }
            else // else, no crosshair
            {
                UserInterface.SetCursor(crosshairTypes.none);
            }
        }
    }


    public void LoadMainMenu()
    {
        StaticsList.destroyAll();
    }*/
}