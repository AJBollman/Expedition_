
// All the stuff the Explorer can do.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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

    private GameObject crosshair;
    private GameObject vignette;
    private CameraOperator cam;
    private float preFOV;

    void Start()
    {
        crosshair = GameObject.Find("Crosshair"); if (!crosshair) throw new System.Exception("Crosshair not found. Make sure there is a gameObject named 'Crosshair'");
        vignette = GameObject.Find("Vignette"); if (!vignette) throw new System.Exception("Vignette not found. Make sure there is a gameObject named 'Vignette'");
        cam = GetComponentInChildren<CameraOperator>(); if (!cam) throw new System.Exception("Camera Container not found. Make sure there is a Camera Container");
        preFOV = cam.defaultFOV;
    }






    // Called every frame.
    void Update()
    {
        /////////////////////   Do raycast drawing stuff.
        // Start a new line in the active Region while the button is down.
        if (Input.GetButtonDown(cameraDrawButton))
        {
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
    }








    /////////////////////////////////////////////////////////   RAYCAST DRAWING
	public static bool cameraDrawAllowed = true;
	public static string cameraDrawButton = "Fire1";
    public static float cameraDrawMaxDistance = 25;

    // Instantiate a new Map Line prefab under active Region.
    private void startCameraLine()
	{
        //Debug.Log(StateController.activeRegion);
		if (!cameraDrawAllowed || !StateController.activeRegion) return;

        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            StateController.activeRegion.addLineToRegion(new Vector3(hit.point.x, hit.point.y, hit.point.z));
        }
	}
    // Add a point to the active Map Line under the Active Region.
    private void addToCameraLine()
	{
		if (!cameraDrawAllowed || !StateController.activeRegion) return;

        var r = Camera.main.ScreenPointToRay(Input.mousePosition);
       RaycastHit hit;
        if (Physics.Raycast(r, out hit, cameraDrawMaxDistance))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green, 0.2f);
            crosshair.SetActive(true);
            StateController.activeRegion.addLinePointToRegion(new Vector3(hit.point.x, hit.point.y, hit.point.z));
        }
        else crosshair.SetActive(false);
    }
    // Tell the active Region to 'sink' the latest Line under the ground.
    private void endCameraLine()
    {
        StateController.activeRegion.sinkLatestLine();
    }




    /////////////////////////////////////////////////////////   UNDO AND REDO LINES
    public static bool undoRedoAllowed = true;
    public static string undoKey = "z";
    public static string redoKey = "x";
    private bool undoRedoPre;

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






    /////////////////////////////////////////////////////////   DRAWING ON MAP






    /////////////////////////////////////////////////////////   DRAWING RED LINE





}
