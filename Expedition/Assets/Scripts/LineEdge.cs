using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(LineRenderer))]
public sealed class LineEdge : MonoBehaviour
{
    public LineVertex from {get; private set;}
    public LineVertex to {get; private set;}

    private LineRenderer lineRenderer;
    private BoxCollider boxCollider;

    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        //SceneManager.MoveGameObjectToScene
    }

    public void setVertices(LineVertex a, LineVertex b) {
        from = a;
        to = b;
        transform.position = Vector3.Lerp(a.transform.position, b.transform.position, 0.5f);
        recalculateLine();
    }

    private void recalculateLine() {
        // recalc boxcollider
        //lineRenderer.SetPosition(0, transform.InverseTransformPoint(from.transform.position));
        //lineRenderer.SetPosition(1, transform.InverseTransformPoint(to.transform.position));
        projectLineVisual();
    }

    // Generate a line of black ink between two lineVertices by projecting onto the surface between them.
    private void projectLineVisual() {
        if(to == null) {Debug.LogWarning("No endpoint"); return;}
        Transform one = from.transform;
        Transform two = to.transform;

        lineRenderer.positionCount = 0; // clear line points.
        lineRenderer.positionCount = Expedition.lineQualityIterations;

        Vector3 projectionStart = one.position; // the start of the line projection.
        Vector3 projectionEnd = two.position; // the end of the line projection.

        // projection point calculation:
        // walk the two projection points away from the surface as far as possible,
        // until the two projection points no longer can 'see' eachother, or the number of 'walks' exceeds the limit.
        for(int y = Expedition.lineProjectionStartDistance; y < Expedition.lineProjectionSolverMaxRange; y++) {
            Vector3 testStart = one.position + (one.rotation * Vector3.up) * y;
            Vector3 testEnd   = two.position + (two.rotation * Vector3.up) * y;

            if(Physics.Linecast(testStart, testEnd, layerMask: Expedition.raycastIgnoreLayers) && Physics.Linecast(testEnd, testStart, layerMask: Expedition.raycastIgnoreLayers)) {
                Debug.DrawLine(testStart, testEnd, Color.red, 10f);
                break;
            }
            else {
                Debug.DrawLine(testStart, testEnd, Color.blue, 10f);
                projectionStart = testStart;
                projectionEnd = testEnd;
            }
        }

        // projecting the line points:
        // break the distance between the first and last projection points into N number of positions, where N = Expedition.lineQualityIterations;
        // from each of these N positions, raycast onto the surface and add a point to the linerenderer there if successful.
        Vector3 lastPos = Vector3.zero;
        for(int x = 0; x < Expedition.lineQualityIterations; x++) {
            RaycastHit hit;
            Vector3 casterPos = Vector3.Lerp(projectionStart, projectionEnd, (float)x / (Expedition.lineQualityIterations - 1) );
            Vector3 maxRangePos = Vector3.Lerp(one.position, two.position, (float)x / (Expedition.lineQualityIterations - 1) );
            Quaternion casterRot = Quaternion.Lerp(one.rotation, two.rotation, (float)x / (Expedition.lineQualityIterations - 1) );

            var range = Vector3.Distance(casterPos, maxRangePos) + Expedition.lineProjectionOvershootDistance;
            // TODO could replace if/else with Linecast...
            if( Physics.Raycast(
                    casterPos, 
                    (casterRot * Vector3.up * -1), 
                    out hit, 
                    range, 
                    layerMask: Expedition.raycastIgnoreLayers
                ) ) {
                Debug.DrawLine(casterPos, hit.point, Color.green, 10f);
                if(lastPos == Vector3.zero || Vector3.Distance(lastPos, hit.point) <= range) {
                    lineRenderer.SetPosition(x, transform.InverseTransformPoint(hit.point + (hit.normal * 0.2f) ));
                    lastPos = hit.point;
                }
            }
            else {
                //Debug.LogError("Raycast point "+x+" failed");
                var makeupHit = casterPos + (casterRot * Vector3.up * -1) * range;
                Debug.DrawLine(casterPos, makeupHit, Color.yellow, 10f);
                if(lastPos == Vector3.zero || Vector3.Distance(lastPos, makeupHit) <= range) {
                    lineRenderer.SetPosition(x, transform.InverseTransformPoint(makeupHit));
                    lastPos = makeupHit;
                }
            }
        }
        // Reduce the number of lineRenderer vertices.
        //lineRenderer.Simplify(0.05f);
    }
}
