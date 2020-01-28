#if false
// Class for map regions

using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
[DisallowMultipleComponent]
public sealed class Region : MonoBehaviour 
{
    [SerializeField] public string regionName {get; private set;}
    
    public static List<Region> allRegionsList {get; private set;}
    public static Region activeRegion {get; private set;}

    private bool isComplete;
    private bool isLocked;
    private GameObject background;
    private Camera cam;
    private SoundPlayer sound;
    private List<GameObject> lines = new List<GameObject>();
    private List<GameObject> redoLines = new List<GameObject>();
    private GameObject startPoint;
    private GameObject endPoint;

    private void Awake()
    {
        background = transform.Find("Map Background").gameObject;
        if (!background) throw new Exception("Region '" + regionName + "' does not have a background!");
        cam = GetComponentInChildren<Camera>();
        if (!cam) throw new Exception("Region '" + regionName + "' does not have a camera!");
        sound = GetComponent<SoundPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == S_Player.Explorer) 
        {
            activeRegion = this;
            Debug.Log("asdf");
            foreach (GameObject l in lines) {l.SetActive(true);} // Show region's lines
            cam.targetTexture = Expedition.mapTexture; // Map's texture shows the region cam's view.
            sound.Play("Enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == S_Player.Explorer)
        {
            activeRegion = null;
            foreach (GameObject l in lines) {l.SetActive(false);} // Hide region's lines
            cam.targetTexture = null;
        }
    }


    // METHODS //////////////////////////////////////////////////////////////////////////
    // Makes sure this region isn't intersecting another region.
    /*private void checkIntersection()
    {
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2, Quaternion.identity);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == tag && hitColliders[i].gameObject != gameObject)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                block.SetColor("_BaseColor", Color.red);
                GetComponent<Renderer>().SetPropertyBlock(block);
                hitColliders[i].gameObject.GetComponent<Renderer>().SetPropertyBlock(block);
                throw new System.Exception("Intersecting regions!");
            }
        }
    }*/

    // Add a new line prefab.
    public bool addLine(Vector3 position)
    {
        if(isLocked) {
            Debug.LogWarning("Can't add line, region is locked.");
            return false;
        }
        else if(lines.Count > 25) {
            Debug.LogWarning("Region can't hold more than 25 lines!");
            return false;
        }
        else {
            lines.Add(Instantiate(Expedition.linePrefab, position, Quaternion.identity, transform));
            redoLines = new List<GameObject>();
            return true;
        }
    }

    // Add a point to the latest line.
    public bool addLinePoint(Vector3 position)
    {
       if(isLocked) {
            Debug.LogWarning("Can't add line point, region is locked.");
            return false;
        }
        else if(lines.Count < 1) {
            throw new System.Exception("Can't add line point, this region has no lines.");
        }
        else {
            return lines[lines.Count - 1].GetComponent<Line>().addPoint(position);
        }
    }

    // Delete and re-instantiate this region's red line.
    // TODO: make sure the startPosition intersects the region's start portal
   /*public void restartRedLine(Vector3 startPosition)
    {
       if(isLocked) {
            Debug.LogWarning("Can't add line, region is locked.");
            return;
        }
        Destroy(redLine); // clear it regardless
        if (startPosition == Vector3.zero) // this happens if the handheld map raycast fails
        {
            Debug.LogWarning("Cannot add red line, invalid position recieved!");
            return;
        }
        redLine = Instantiate(Expedition.templateRedline, startPosition, Quaternion.identity, transform);
    }

    // Add a point to this region's red line.
    public void addRedLinePoint(Vector3 position)
    {
        if(isLocked) {
            Debug.LogWarning("Can't add line, region is locked.");
            return;
        }
        if (redLine == null)
        {
            Debug.LogWarning("Cannot add red line point; this region has no red line!");
            return;
        }
        redLine.GetComponent<Line>().addPoint(position);
    }*/

    // Sink or rise all lines in this region.
    public void setSinkOfAllLines(bool sink) {
        foreach(GameObject l in lines) {
            l.GetComponent<Line>().sinkLine(sink);
        }
    }

    // TODO return something pertaining to portals ---------------------------------------------- 
    public Portal getActivePortal() {
        return startPoint.GetComponent<Portal>();
    }

    // Hide or show all this region's lines.
    public void setLineVisibility(bool show) {
        foreach (GameObject l in lines) {
            l.SetActive(show);
        }
    }

    // Remove the latest line and add it to the undo list.
    public void undoLine()
    {
       if(isLocked) {
            Debug.LogWarning("Can't undo line, region is locked.");
            return;
        }
        if (lines.Count == 0)
        {
            Debug.LogWarning("Cannot undo, no lines left!");
            return;
        }
        redoLines.Add(lines[lines.Count - 1]);
        lines[lines.Count - 1].SetActive(false);
        lines.RemoveAt(lines.Count - 1);
    }

    // Push the latest line in the undo list back into the region.
    public void redoLine()
    {
       if(isLocked) {
            Debug.LogWarning("Can't redo line, region is locked.");
            return;
        }
        if (redoLines.Count == 0)
        {
            Debug.LogWarning("Nothing to redo.");
            return;
        }
        redoLines[redoLines.Count - 1].SetActive(true);
        lines.Add(redoLines[redoLines.Count - 1]);
        redoLines.RemoveAt(redoLines.Count - 1);
    }

    // 
    public Vector3 raycastFromRegionCamera(Vector2 pos) {
        var camRay = cam.ViewportPointToRay(pos);
        RaycastHit camHit;
        if (Physics.Raycast(camRay, out camHit, 1000f, layerMask: Expedition.raycastIgnoreLayers)) {
            return camHit.point;
        }
        else return Vector3.zero;
    }

    // Check if this region is complete.
    public bool getIsComplete()
    {
        return isComplete;
    }

    // Check if this region can be marked as complete.
    public bool checkCompletion()
    {
        return false;
    }

}
#endif