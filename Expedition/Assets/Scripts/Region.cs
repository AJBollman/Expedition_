
// Class for map regions


using System.Collections;
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

    //private List<Vector3> redLine = new List<Vector3>();
    private GameObject background;
    private Camera cam;
    private static GameObject templateLine;
    private static GameObject templateRedLine;
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
        templateLine = GameObject.Find("Line");
        templateRedLine = GameObject.Find("RedLine");
        if (!templateLine) throw new System.Exception("Region could not find line prefab. Make sure a 'Line' prefab is in the scene!");
        if (!templateRedLine) throw new System.Exception("Region could not find red line prefab. Make sure a 'RedLine' prefab is in the scene!");

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

    // Set the active region upon entering.
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            StateController.activeRegion = this;
            StateController.activeRegionCamera = GetComponentInChildren<Camera>();
            activateCamera(true);
            //Debug.Log("Active region: " + StateController.activeRegion.name);
            foreach (GameObject l in lines) {l.SetActive(true);} // Show lines.
        }
    }

    // Set active region to null on exit. If the player just walked into a new region, it'll become active a moment after this.
    void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
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
        if (transform.childCount - 2 >= lineLimit)
        {
            Debug.LogWarning("Cannot add line to region '" + name + "', limit reached!");
            return;
        }
        GameObject createdLine = Instantiate(templateRedLine, pos, Quaternion.identity, transform);
        redLines.Add(createdLine);

    }

    // Add a new point to this region's latest red Line.
    public void addRedLinePointToRegion(Vector3 pos)
    {
        if (redLines.Count < 1)
        {
            Debug.LogWarning("Cannot add redline points; this region has no active line!");
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
}
