
using System;
using UnityEngine;

/// <summary> All the stuff the Explorer can do. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(S_Movement))]
[RequireComponent(typeof(SoundPlayer))]
public sealed class S_Player : MonoBehaviour
{
    // TESTING
    private GameObject indicator;
    private GameObject lastIndicator;

    [SerializeField] private Gradient trueGradient;
    [SerializeField] private Gradient falseGradient;

    private Vector3 indicGoalPos;
    private Quaternion indicGoalRot;
    [SerializeField] private float indicPosSmooth = 20f;
    [SerializeField] private float indicRotSmooth = 10f;

    [SerializeField] private GameObject indicatorEnd;
    [SerializeField] private GameObject lastIndicatorEnd;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject line;

    [SerializeField] private float miniMapZoom = 32f;
    [SerializeField] private float fullMapZoom = 64f;
    [SerializeField] private GameObject minimapCam;

    public static bool isInQuestZone;
    private GameObject pickupScroll;
    private GameObject handheldScrollTarget;
    private Vector3 pickupScrollGoalPos;
    private Quaternion pickupScrollGoalRot;
    public float handheldSmooth = 40f;



    /// <summary> The Explorer gameobject. </summary>
    public static GameObject Explorer {get; private set;}

    /// <summary> Maximum distance for the raycasting. </summary>
    public float maxDistance { get; private set; } = 15f;

    [SerializeField] private bool drawDebugLines;

    private static RaycastHit lastRaycastHit;
    private LineVertex lastVert;
    
    private static GameObject handheldContainer;
    private static GameObject handheldGoal;
    private static GameObject fullMapGoal;
    private Vector3 mapGoalPos;
    private Quaternion mapGoalRot;
    private bool isFullMap;
    private Vector3 mapCamGoalPos;



    #region preRework
    public static bool cameraDrawAllowed = true;
    public string cameraDrawButton = "Fire1";
    public float cameraDrawMaxDistance = 15;
    public LayerMask raycastIgnoreLayers;
    public float coordPlaneSizeFudgeFactor = 1f;

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
            if(Explorer == null) throw new System.Exception("Player could not find player object");
            /*sound = GetComponent<SoundPlayer>();
            jumpLoopSrc = gameObject.AddComponent<AudioSource>();
            jumpLoopSrc.clip = jumpLoop;
            jumpLoopSrc.loop = true;
            drawLoopSrc = gameObject.AddComponent<AudioSource>();
            drawLoopSrc.clip = drawLoop;
            drawLoopSrc.loop = true;*/

            indicator = GameObject.Find("Vertex Indicator");
            lastIndicator = GameObject.Find("Last Vertex Indicator");
            line = GameObject.Find("Indicator Line");
            handheldContainer = GameObject.Find("Handheld Items Container");
            handheldGoal = GameObject.Find("Hand Map Goal");
            fullMapGoal = GameObject.Find("Full Map Goal");
            minimapCam = GameObject.Find("Minimap Cam");
            lineRenderer = line.GetComponent<LineRenderer>();
            indicatorEnd = indicator.transform.Find("penEnd").gameObject;
            lastIndicatorEnd = lastIndicator.transform.Find("penEnd").gameObject;

            if(indicator == null) throw new Exception("Player has no vertex indicator");
            if(lastIndicator == null) throw new Exception("Player has no last indicator");
            if(line == null) throw new Exception("Player has no indicator line");
            if(handheldContainer == null) throw new Exception("Player has no handheld container");
            if(handheldGoal == null) throw new Exception("Player has no handheld goal");
            if(fullMapGoal == null) throw new Exception("Player has no fullmap goal");
            if(minimapCam == null) throw new Exception("Player has no minimap cam");
            if(lineRenderer == null) throw new Exception("Line has no line renderer");
            if(indicatorEnd == null) throw new Exception("indicator has no indicatorEnd");
            if(lastIndicatorEnd == null) throw new Exception("lastindicator has no indicatorEnd");

            lineRenderer.positionCount = 2;
            lastVert = null;
            lastIndicator.SetActive(false);
            line.SetActive(false);
            mapCamGoalPos = minimapCam.transform.position;

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
    
    // Called every frame.
    void Update()
    {

        if(viewRaycast(maxDistance) && !isFullMap) {
            indicGoalPos = lastRaycastHit.point;
            indicGoalRot = Quaternion.FromToRotation(Vector3.up, lastRaycastHit.normal);
        }
        indicator.transform.position = Vector3.Lerp(indicator.transform.position, indicGoalPos, Time.deltaTime * indicPosSmooth);
        indicator.transform.rotation = Quaternion.Slerp(indicator.transform.rotation, indicGoalRot, Time.deltaTime * indicRotSmooth);

        lineRenderer.SetPosition(0, lastIndicatorEnd.transform.position);
        lineRenderer.SetPosition(1, indicatorEnd.transform.position);

        if(lastVert != null) {
            if( Physics.Linecast(indicatorEnd.transform.position, lastIndicatorEnd.transform.position, layerMask:Expedition.raycastIgnoreLayers) ) {
                cameraDrawAllowed = false;
                lineRenderer.colorGradient = falseGradient;
            }
            else {
                cameraDrawAllowed = true;
                lineRenderer.colorGradient = trueGradient;
            }
        }

        if(Input.GetKeyDown(KeyCode.E)) {
            if(lastVert != null) {
                lastVert = null;
                lastIndicator.SetActive(false);
                line.SetActive(false);
            }
            else if(fullMapAllowed) {
                isFullMap = !isFullMap;
                Expedition.CameraOperator.AllowInput = !isFullMap;
                Expedition.Movement.AllowInput = !isFullMap;
                minimapCam.GetComponent<Camera>().orthographicSize = (isFullMap) ? fullMapZoom : miniMapZoom;
            }
        }

        if(isFullMap) {
            mapCamGoalPos += new Vector3(
                (Input.GetAxis("Horizontal")),
                0,
                (Input.GetAxis("Vertical"))
            );
            Debug.Log((Input.GetAxis("Vertical")));
        }
        else {
            mapCamGoalPos = new Vector3(
                S_Player.Explorer.transform.position.x,
                minimapCam.transform.position.y,
                S_Player.Explorer.transform.position.z
            );
        }

        minimapCam.transform.position = Vector3.Lerp(minimapCam.transform.position, mapCamGoalPos, Time.deltaTime * 10);

        // When button is pressed, 
        if (Input.GetButtonDown(cameraDrawButton)) {}

        // While the button is down, add new points to the line that was just made.
        if (Input.GetButton(cameraDrawButton)) {}

        mapGoalRot = (isFullMap) ? fullMapGoal.transform.rotation : handheldGoal.transform.rotation;
        mapGoalPos = (isFullMap) ? fullMapGoal.transform.position : handheldGoal.transform.position;

        // When the button is released, "end" the line.
        // For now, that just means sink the line below the ground.
        if (Input.GetButtonUp(cameraDrawButton) && cameraDrawAllowed)
        {
            LineVertex newVert = LineVertex.SpawnVertex(indicGoalPos, indicGoalRot);
            if(lastVert != null) {
                LineVertex.connectVertices(lastVert, newVert);
            }
            lastVert = newVert;
            lastIndicator.SetActive(true);
            line.SetActive(true);
            lastIndicator.transform.position = indicator.transform.position;
            lastIndicator.transform.rotation = indicator.transform.rotation;
            //if(endCameraLine(true)) isCameraDrawing = false;
        }

        // Follow-through / delayed motion for hand-held objects
        handheldContainer.transform.rotation = Quaternion.Lerp(
            handheldContainer.transform.rotation,
            mapGoalRot,
            Time.deltaTime * handheldSmooth
        );
        handheldContainer.transform.position = Vector3.Lerp(
            handheldContainer.transform.position,
            mapGoalPos,
            Time.deltaTime * handheldSmooth * 0.5f
        );

        /*if (!mapIsFull) isRedLineMode = false;
        Vector3 targetPos;
        if (Expedition.getActiveRegion() == null)
        {
            targetPos = new Vector3(0, -0.25f, -0.25f);
        }
        else targetPos = (mapIsFull) ? fullMapPos : miniMapPos;
        handMap.transform.localPosition = Vector3.Lerp(
            handMap.transform.localPosition,
            targetPos,
            Time.deltaTime * mapFullscreenTransitionTime
        );
        if (!Expedition.getActiveRegion() || !isCameraDrawing) drawLoopSrc.Stop();


        // Player script does nothing in main menu.
        if (cachedGS == gameStates.menu) return;

        ////////////////////    opening the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var isPaused = !PauseMenu.activeSelf;
            Expedition.setState(isPaused ? gameStates.paused : gameStates.normal);

            PauseMenu.SetActive(isPaused);
            //cam.enableControls = !isPaused;
            if (isPaused)
            {
                //if (isRedLineMode) endRedLine();
                if (isCameraDrawing) endCameraLine(false);
                redvignette.SetActive(false);
                vignette.SetActive(false);
            }
            cachedGS = Expedition.getState();
        }

        if (cachedGS != gameStates.normal && cachedGS != gameStates.redline) return;

        checkInteract();

        /////////////////////   Do raycast drawing stuff.
        // Start a new line/redline in the active Region while the button is down.
        if (Input.GetButtonDown(cameraDrawButton))
        {
            if (isRedLineMode) return;//startRedLine();
            else
            {
                isCameraDrawing = true;
                startCameraLine();
                //cam.defaultFOV = preFOV + 5f; // widen FOV a bit while drawing.
                //cam.maxFOVTweak = 0;
                undoRedoPre = undoRedoAllowed;
            }
            undoRedoAllowed = false; // Disable undo/redo while drawing.
        }

        // While the button is down, add new points to the line that was just made.
        if (Input.GetButton(cameraDrawButton))
        {
            if (isRedLineMode) return;//addToRedLine();
            else addToCameraLine();
        }

        // When the button is released, "end" the line.
        // For now, that just means sink the line below the ground.
        if (Input.GetButtonUp(cameraDrawButton))
        {
            if (isRedLineMode) return;//endRedLine();
            else
            {
                isCameraDrawing = false;
                endCameraLine(true);
                //cam.defaultFOV = preFOV; // Reset FOV.
                //cam.maxFOVTweak = 5f;
            }
            undoRedoAllowed = undoRedoPre; // Set the undo/redo allowed back to what it was.
        }
        vignette.SetActive(Input.GetButton(cameraDrawButton)); // Camera vignette is visible as long as the mouse is down.
        redvignette.SetActive(isRedLineMode); // Camera vignette is visible as long as the mouse is down.

        /////////////////////   Do undo/redo stuff.
        if (Input.GetKeyDown(undoKey)) undoLastLine();
        if (Input.GetKeyDown(redoKey)) redoLastLine();
        if (Input.GetKeyDown(fullKey)) toggleFullMap();
        if (Input.GetKeyDown(redLinekey) && mapIsFull && Expedition.getPortalOfActiveRegion() != null) isRedLineMode = !isRedLineMode;
        if (isRedLineMode) Expedition.setState(gameStates.redline);
        else Expedition.setState(gameStates.normal);

        ////////////////////   Toggle map rotation.
        if (Input.GetKeyDown(toggleMapRotKey)) toggleMapRot();
        */
    }
    #endregion



    #region [Methods]
    private bool viewRaycast(float range) {
        if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out RaycastHit hit, range, layerMask: Expedition.raycastIgnoreLayers)) {
            // Optionally, show the raycast in scene view.
            if(drawDebugLines) Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            lastRaycastHit = hit;
            return true;
        }
        else return false;
    }

    /*public bool startCameraLine() {
        if (!cameraDrawAllowed) { Debug.LogWarning("Line drawing not allowed!"); return false; }
        if(Region.activeRegion == null) { Debug.LogWarning("Can't start line, no active region!"); return false; }

        if(viewRaycast(maxDistance)) {
            return Region.activeRegion.addLine(lastRaycastHit.point);
        }
        else { Debug.LogWarning("Surface is out of range ("+maxDistance+")"); return false; }
    }

    public bool addLinePoint() {
        if (!cameraDrawAllowed) { Debug.LogWarning("Line drawing not allowed!"); return false; }
        if(Region.activeRegion == null) { Debug.LogWarning("Can't add point, no active region!"); return false; }

        if(viewRaycast(maxDistance)) {
            //if (!drawLoopSrc.isPlaying) drawLoopSrc.Play();
            return Region.activeRegion.addLinePoint(lastRaycastHit.point);
        }
        else { Debug.LogWarning("Surface is out of range ("+maxDistance+")"); return false; }
    }

    public bool endCameraLine(bool playSound) {
        if(Region.activeRegion == null) { Debug.LogWarning("Can't end line, no active region!"); return false; }

        //if (playSound) sound.Play("DrawStop");
        Region.activeRegion.setSinkOfAllLines(true);
        return true;
    }*/
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