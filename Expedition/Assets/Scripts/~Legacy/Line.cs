#if false
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
    public List<Vector3> rawLineData;
    public bool sunken = false;
    public float sinkAmount = 0.05f;

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
    public bool addPoint(Vector3 pos)
    {
        pos.y += 0.05f; // raise Y pos a tiny bit, for visibility.
        var cpos = transform.InverseTransformPoint(pos);
        if (Vector3.Distance(lastPoint, cpos) < 0.010f) { return false; }
        {

            //Debug.DrawLine(transform.position, pos, Color.green, 0.5f);
            if (lineData.Count > maxLinePoints)
            {
                lineData.RemoveAt(lineData.Count - 1);
                rawLineData.RemoveAt(lineData.Count - 1);
            }
            lineData.Insert(0, cpos);
            rawLineData.Insert(0, pos);
            linerender.SetPositions(lineData.ToArray());
            lastPoint = cpos;
            return true;
        }
    }

    // 'Sink' this line below the map so it's only visible on the map and not in the level.
    public void sinkLine(bool tf)
    {
        if (tf == sunken) return; // it's not being changed.

        var prePos = transform.localPosition;
        transform.localPosition = new Vector3(
            prePos.x,
            prePos.y + (tf ? -sinkAmount : sinkAmount),
            prePos.z
        );
        sunken = tf;
    }

    public List<Vector3> getPoints()
    {
        return rawLineData;
    }
}
#endif