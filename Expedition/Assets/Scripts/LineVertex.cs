using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LineVertex : MonoBehaviour
{

    #region [Public]
    public bool isRed { get => _isRed; }
    public LineEdge getEdge { get => Edge; }
    [SerializeField] private bool _isRed;
    public bool isRedLineFinalized { get; private set; }
    #endregion

    #region [Private]
    private LineEdge Edge;
    #endregion



    #region [Events]
    private void Awake() {
        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByName("Overlay"));
    }

    private void OnDestroy() {
        // so we don't leave floating line edges
        if(Edge != null) Destroy(Edge.gameObject);
    }
    #endregion

    #region [Methods]
    public static LineVertex SpawnVertex(Vector3 pos, Quaternion rot, bool isRed = false) {
        var reference = (isRed) ? Expedition.RedLineVertexPrefab : Expedition.LineVertexPrefab;
        GameObject newVertObj = GameObject.Instantiate( reference, pos, rot );
        return newVertObj.GetComponent<LineVertex>();
    }

    public static void ConnectVertices(LineVertex a, LineVertex b) {
        if(a == null || b == null || a == b) throw new System.Exception("Cannot connect invalid verticies");
        if(a.isRed != b.isRed) throw new System.Exception("Cannot connect unlike vertices");

        GameObject newEdgeObj = GameObject.Instantiate(
            (a.isRed) ? Expedition.RedLineEdgePrefab : Expedition.LineEdgePrefab, 
            Vector3.Lerp(a.transform.position, b.transform.position, 0.5f), 
            Quaternion.identity
        );
        LineEdge newEdge = newEdgeObj.GetComponent<LineEdge>();
        newEdge.SetVertices(a, b);
        a.Edge = newEdge;
        b.Edge = newEdge;
    }

    public void Move(Vector3 pos) {
        transform.position = pos;
        if(Edge != null) Edge.RecalculateLine();
    }
    #endregion
}
