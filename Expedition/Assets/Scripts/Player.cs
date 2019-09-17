
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
    public Vector3 fullMapPos = new Vector3(    0,     0, 0.12f);
    public Vector3 miniMapPos = new Vector3(0.26f, -0.1f, 0.23f);

    public static bool mapIsFull;

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
    private GameObject handMap;
    private CameraOperator cam;
    private Movement mv;
    private float preFOV;
    private float preFOVT;
    private bool undoRedoPre;

    void Start()
    {
        crosshair = GameObject.Find("Crosshair"); if (!crosshair) throw new System.Exception("Crosshair not found. Make sure there is a gameObject named 'Crosshair'");
        vignette = GameObject.Find("Vignette"); if (!vignette) throw new System.Exception("Vignette not found. Make sure there is a gameObject named 'Vignette'");
        cam = GameObject.Find("CameraContainer").GetComponent<CameraOperator>(); if (!cam) throw new System.Exception("Camera Container not found. Make sure there is a Camera Container");
        handMap = GameObject.Find("HandMap Offset"); if (!handMap) throw new System.Exception("Hand Map Offset not found. Make sure there is a Hand Map Offset");
        preFOV = cam.defaultFOV;
        preFOVT = cam.maxFOVTweak;
        mv = GetComponent<Movement>();
    }






    // Called every frame.
    void Update()
    {
        /////////////////////   Do raycast drawing stuff.
        // Start a new line in the active Region while the button is down.
        if (Input.GetButtonDown(cameraDrawButton))
        {
            isCameraDrawing = true;
            startCameraLine();
            cam.defaultFOV = preFOV - 5f; // narrow FOV a bit while drawing.
            cam.maxFOVTweak = 0;
            undoRedoPre = undoRedoAllowed;
            undoRedoAllowed = false; // Disable undo/redo while drawing.
        }

        // While the button is down, add new points to the line that was just made.
        if (Input.GetButton(cameraDrawButton)) addToCameraLine();

        // When the button is released, "end" the line.
        // For now, that just means sink the line below the ground.
        if (Input.GetButtonUp(cameraDrawButton))
        {
            isCameraDrawing = false;
            endCameraLine();
            crosshair.SetActive(false); // Hide crosshair.
            cam.defaultFOV = preFOV; // Reset FOV.
            cam.maxFOVTweak = 5f;
            undoRedoAllowed = undoRedoPre; // Set the undo/redo allowed back to what it was.
        }
        vignette.SetActive(Input.GetButton(cameraDrawButton)); // Camera vignette is visible as long as the mouse is down.

        /////////////////////   Do undo/redo stuff.
        if (Input.GetKeyDown(undoKey)) undoLastLine();
        if (Input.GetKeyDown(redoKey)) redoLastLine();
        if (Input.GetKeyDown(fullKey)) toggleFullMap();

        Vector3 targetPos = (mapIsFull) ? fullMapPos : miniMapPos;
        handMap.transform.localPosition = Vector3.Lerp(
            handMap.transform.localPosition,
            targetPos,
            Time.deltaTime * mapFullscreenTransitionTime
        );
    }








    /////////////////////////////////////////////////////////   RAYCAST DRAWING
    // Instantiate a new Map Line prefab under active Region.
    private void startCameraLine()
	{
        //Debug.Log(StateController.activeRegion);
		if (!cameraDrawAllowed || !StateController.activeRegion) return;

        var r = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance, layerMask:raycastIgnoreLayers))
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
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance, layerMask:raycastIgnoreLayers))
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
        else StateController.setState(gameStates.normal);      // todo
    }





    /////////////////////////////////////////////////////////   DRAWING ON MAP






    /////////////////////////////////////////////////////////   DRAWING RED LINE





}
