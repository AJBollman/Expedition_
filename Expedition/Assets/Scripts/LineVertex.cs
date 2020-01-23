using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineVertex : MonoBehaviour
{
    private void Awake() {
        //SceneManager.MoveGameObjectToScene
    }

    public static LineVertex SpawnVertex(Vector3 pos, Quaternion rot) {
        GameObject newVertObj = GameObject.Instantiate(Expedition.lineVertexPrefab, pos, rot);
        //SceneManager.MoveGameObjectToScene
        return newVertObj.GetComponent<LineVertex>();
    }

    public static void connectVertices(LineVertex a, LineVertex b) {
        if(a == null || b == null || a == b) throw new System.Exception("Cannot connect invalid verticies");

        GameObject newEdgeObj = GameObject.Instantiate(
            Expedition.lineEdgePrefab, 
            Vector3.Lerp(a.transform.position, b.transform.position, 0.5f), 
            Quaternion.identity
        );
        LineEdge newEdge = newEdgeObj.GetComponent<LineEdge>();
        newEdge.setVertices(a, b);
        //return newEdge.GetComponent<Edge>();
    }
}
