// All the stuff the Explorer can do.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool cameraDrawAllowed = true;
    public string cameraDrawButton = "Fire1";
    public float cameraDrawMaxDistance = 15;
    public LayerMask raycastIgnoreLayers;

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

    public static bool mapIsFull;
    public static bool mapSpins;
    public static bool isRedLineMode;

    // https://gamedev.stackexchange.com/a/116010 singleton pattern.
    private static Player _instance;
    private static Player Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        { Destroy(this.gameObject); }
        else
        { _instance = this; }
    }

    public bool isCameraDrawing;
    public Vector3 lastRaycastHit;
    private GameObject crosshair;
    private GameObject vignette;
    private GameObject redvignette;
    private GameObject handMap;
    private CameraOperator cam;
    private GameObject coordOrigin0;
    private GameObject coordOrigin1;
    private Vector3 coordPlaneBounds;

    private Movement mv;
    private float preFOV;
    private float preFOVT;
    private bool undoRedoPre;

    void Start()
    {
        crosshair = GameObject.Find("Crosshair"); if (!crosshair) throw new System.Exception("Crosshair not found. Make sure there is a gameObject named 'Crosshair'");
        vignette = GameObject.Find("Vignette"); if (!vignette) throw new System.Exception("Vignette not found. Make sure there is a gameObject named 'Vignette'");
        redvignette = GameObject.Find("Redline Vignette"); if (!redvignette) throw new System.Exception("Redline Vignette not found. Make sure there is a gameObject named 'Redline Vignette'");
        cam = GameObject.Find("CameraContainer").GetComponent<CameraOperator>(); if (!cam) throw new System.Exception("Camera Container not found. Make sure there is a Camera Container");
        handMap = GameObject.Find("HandMap Offset"); if (!handMap) throw new System.Exception("Hand Map Offset not found. Make sure there is a 'Hand Map Offset'");
        coordOrigin0 = GameObject.Find("CoordOrigin0"); if (!coordOrigin0) throw new System.Exception("Hand Map UpperLeft Coordinate origin not found. Make sure there is a 'coordOrigin0'");
        coordOrigin1 = GameObject.Find("CoordOrigin1"); if (!coordOrigin1) throw new System.Exception("Hand Map BottomRight Coordinate origin not found. Make sure there is a 'coordOrigin1'");

        preFOV = cam.defaultFOV;
        preFOVT = cam.maxFOVTweak;
        mv = GetComponent<Movement>();
        coordPlaneBounds = new Vector3(
            Mathf.Abs(coordOrigin0.transform.position.x - coordOrigin1.transform.position.x),
            Mathf.Abs(coordOrigin0.transform.position.y - coordOrigin1.transform.position.y),
            Mathf.Abs(coordOrigin0.transform.position.z - coordOrigin1.transform.position.z)
        );
    }






    // Called every frame.
    void Update()
    {
        /////////////////////   Do raycast drawing stuff.
        // Start a new line/redline in the active Region while the button is down.
        if (Input.GetButtonDown(cameraDrawButton))
        {
            if (isRedLineMode)
            {
                startRedLine();
            }
            else
            {
                isCameraDrawing = true;
                startCameraLine();
                cam.defaultFOV = preFOV - 5f; // narrow FOV a bit while drawing.
                cam.maxFOVTweak = 0;
                undoRedoPre = undoRedoAllowed;
            }
            undoRedoAllowed = false; // Disable undo/redo while drawing.
        }

        // While the button is down, add new points to the line that was just made.
        if (Input.GetButton(cameraDrawButton))
        {
            if (isRedLineMode)
            {
                addToRedLine();
            }
            else addToCameraLine();
        }

        // When the button is released, "end" the line.
        // For now, that just means sink the line below the ground.
        if (Input.GetButtonUp(cameraDrawButton))
        {
            if (isRedLineMode)
            {
                endRedLine();
            }
            else
            {
                isCameraDrawing = false;
                endCameraLine();
                crosshair.SetActive(false); // Hide crosshair.
                cam.defaultFOV = preFOV; // Reset FOV.
                cam.maxFOVTweak = 5f;
            }
            undoRedoAllowed = undoRedoPre; // Set the undo/redo allowed back to what it was.
        }
        vignette.SetActive(Input.GetButton(cameraDrawButton)); // Camera vignette is visible as long as the mouse is down.
        redvignette.SetActive(isRedLineMode); // Camera vignette is visible as long as the mouse is down.

        /////////////////////   Do undo/redo stuff.
        if (Input.GetKeyDown(undoKey)) undoLastLine();
        if (Input.GetKeyDown(redoKey)) redoLastLine();
        if (Input.GetKeyDown(fullKey)) toggleFullMap();
        if (Input.GetKeyDown(redLinekey) && mapIsFull) isRedLineMode = !isRedLineMode;
        if (!mapIsFull) isRedLineMode = false;

        ////////////////////   Fullscreen map.
        Vector3 targetPos = (mapIsFull) ? fullMapPos : miniMapPos;
        handMap.transform.localPosition = Vector3.Lerp(
            handMap.transform.localPosition,
            targetPos,
            Time.deltaTime * mapFullscreenTransitionTime
        );

        ////////////////////   Toggle map rotation.
        if (Input.GetKeyDown(toggleMapRotKey)) toggleMapRot();
    }








    /////////////////////////////////////////////////////////   RAYCAST DRAWING
    // Instantiate a new Map Line prefab under active Region.
    private void startCameraLine()
    {
        if (!cameraDrawAllowed || !StateController.activeRegion) return;

        var r = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance, layerMask: raycastIgnoreLayers))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            Vector3 newHit = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            lastRaycastHit = newHit;
            StateController.activeRegion.addLineToRegion(newHit);
        }
    }
    // Add a point to the active Map Line under the Active Region.
    private void addToCameraLine()
    {
        if (!cameraDrawAllowed || !StateController.activeRegion) return;

        var r = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance, layerMask: raycastIgnoreLayers))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            crosshair.SetActive(true);
            Vector3 newHit = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            lastRaycastHit = newHit;
            StateController.activeRegion.addLinePointToRegion(newHit);
        }
        else crosshair.SetActive(false);
    }
    // Tell the active Region to 'sink' the latest Line under the ground.
    private void endCameraLine()
    {
        if (!cameraDrawAllowed || !StateController.activeRegion) return;
        StateController.activeRegion.sinkLatestLine();
    }




    /////////////////////////////////////////////////////////   UNDO AND REDO LINES
    // Undoes the last drawn Line in the active Region.
    private void undoLastLine()
    {
        if (!undoRedoAllowed) return;
        StateController.activeRegion.undoLine();
    }
    // Redoes the last drawn Line in the active Region.
    private void redoLastLine()
    {
        if (!undoRedoAllowed) return;
        StateController.activeRegion.redoLine();
    }





    /////////////////////////////////////////////////////////   FULLSCREEN MAP
    public void toggleFullMap()
    {
        mapIsFull = !mapIsFull;
        cam.maxFOVTweak = (mapIsFull) ? 0f : preFOVT;
        cameraDrawAllowed = !mapIsFull;
        undoRedoAllowed = !mapIsFull;
        cam.enableControls = !mapIsFull;
        mv.moveAllowed = !mapIsFull;
        if (mapIsFull) StateController.setState(gameStates.fullmap);
        else StateController.setState(gameStates.normal);    // todo
    }





    /////////////////////////////////////////////////////////   TOGGLE MAP SPINNING
    ///// todo make this an option in a future options menu.
    public void toggleMapRot()
    {
        mapSpins = !mapSpins;

    }





    /////////////////////////////////////////////////////////   DRAWING ON MAP
    private Vector3 screenRaycastOntoMap()
    {
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.collider.gameObject.name == "HandMap")
            {
                Vector2 pos = new Vector2(
                    Mathf.Abs(coordOrigin0.transform.position.y - hit.point.y) / coordPlaneBounds.y,
                    Mathf.Abs(coordOrigin1.transform.position.z - hit.point.z) / coordPlaneBounds.z
                );

                if(StateController.activeRegionCamera == null) throw new System.Exception("No region cam to raycast from!");
                var camRay = StateController.activeRegionCamera.ViewportPointToRay(pos);
                RaycastHit camHit;
                if (Physics.Raycast(camRay, out camHit, Mathf.Infinity, layerMask: raycastIgnoreLayers))
                {
                    Debug.DrawLine(StateController.activeRegionCamera.transform.position, camHit.point);
                    return camHit.point;
                    //StateController.activeRegion.addLinePointToRegion(newHit);
                }
            }
        }
        return Vector3.zero;
    }





    /////////////////////////////////////////////////////////   DRAWING RED LINE
    // Instantiate a new Red Line prefab under active Region.
    private void startRedLine()
    {
        if (StateController.activePortal != null)
        {
            StateController.activeRegion.addRedLineToRegion(screenRaycastOntoMap());
        }
        else Debug.LogWarning("Can't start redline, not in a portal!");
    }
    // Add a point to the active Red Line under the Active Region.
    private void addToRedLine()
    {
        if (StateController.activePortal != null)
        {
            StateController.activeRegion.addRedLinePointToRegion(screenRaycastOntoMap());
        }
        else Debug.LogWarning("Can't add to redline, not in a portal!");
    }
    private void endRedLine()
    {
        if (StateController.activePortal != null)
        {
            StateController.startTraveller();
            toggleFullMap();
        }
        else Debug.LogWarning("Can't end redline, not in a portal!");
    }




}