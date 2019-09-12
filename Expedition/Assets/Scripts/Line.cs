
// This class lets you add points to the lineRenderer.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public int maxLinePoints = 100;
    private LineRenderer linerender;
    public List<Vector3> lineData;
    private Vector3 lastPoint = Vector3.zero;
    public bool sunken = false;

    void Awake()
    {
        linerender = GetComponent<LineRenderer>();
        linerender.positionCount = maxLinePoints;
        sinkLine(sunken);
    }

    // not using this as of right now
    /*public void updatePoints(List<Vector3> points)
    {
        lineData = new List<Vector3>();
        linerender.positionCount = 0;
        linerender.positionCount = maxLinePoints;
    }*/

    // Self-explanatory; add a point to the line.
    // Will check to make sure it's not too close.
    public void addPoint(Vector3 pos)
    {
        pos = transform.InverseTransformPoint(pos);
        if (Vector3.Distance(lastPoint, pos) < 0.010f) { return; }
        {
            Debug.DrawLine(transform.position, pos, Color.green, 0.5f);
            if (lineData.Count > maxLinePoints)
            {
                lineData.RemoveAt(lineData.Count - 1);
            }
            lineData.Insert(0, pos);
            linerender.SetPositions(lineData.ToArray());
            lastPoint = pos;
        }
    }

    // 'Sink' this line below the map so it's only visible on the map and not in the level.
    public void sinkLine(bool tf)
    {
        if (tf == sunken) return; // it's not being changed.

        var prePos = transform.localPosition;
        transform.localPosition = new Vector3(
            prePos.x,
            prePos.y + (tf ? -0.25f : 0.25f),
            prePos.z
        );
        sunken = tf;
    }
}
