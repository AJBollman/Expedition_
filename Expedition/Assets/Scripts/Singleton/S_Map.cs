
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary> Handles the player's ability to use the map. </summary>
[DisallowMultipleComponent]
public sealed class S_Map : MonoBehaviour
{

    #region [Public]
    /// <summary> Set wether the character can draw lines. </summary>
    public bool IsFullMap { get => IsFullMap; set => _isFullMap = value; }
    public bool IsMapVisible { get => _isMapVisible; set => _isMapVisible = value; }
    public LineVertex LastRedlineVertex { get => _ActiveRedline[_ActiveRedline.Count - 1]; }
    #endregion



    #region [Private]
    [SerializeField] private float _mapSizeMini = 32f;
    [SerializeField] private float _mapSizeFull = 64f;
    [SerializeField] private float _smoothHandheldPosition = 40f;
    private List<LineVertex> _ActiveRedline = new List<LineVertex>();
    private GameObject _MapCamera;
    private GameObject _HandheldContainer;
    private GameObject _HandheldPos;
    private GameObject _FullmapPos;
    private GameObject _HiddenMapPos;
    private Vector3 _goalMapCameraPos;
    private Vector3 _goalHandheldMapPos;
    private Quaternion _goalHandheldMapRot;
    private bool _isFullMap;
    private bool _isMapVisible = true;
    #endregion



    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    #region [Events]
    private void OnEnable() {
        instance = this;
        try {
            _HandheldContainer = GameObject.Find("Handheld Items Container");
            _HandheldPos = GameObject.Find("Hand Map Goal");
            _FullmapPos = GameObject.Find("Full Map Goal");
            _HiddenMapPos = GameObject.Find("Hidden Map Goal");
            _MapCamera = GameObject.Find("Map Camera");

            if(_HandheldContainer == null) throw new Exception("Could not find Handheld Items Container");
            if(_HandheldPos == null) throw new Exception("Could not find Hand Map Goal");
            if(_FullmapPos == null) throw new Exception("Could not find Full Map Goal");
            if(_HiddenMapPos == null) throw new Exception("Could not find Hidden Map Goal");
            if(_MapCamera == null) throw new Exception("Could not find Map Camera");

            _goalMapCameraPos = _MapCamera.transform.position;

            isReady = true;
        }
        catch(Exception e) {
            isReady = false;
            enabled = false;
            Debug.LogException(e);
        }
    }

    private void Update() {
        // stuff to do always:
        
        // set lerp goals.
        if(!_isMapVisible) {
            _goalHandheldMapPos = _HiddenMapPos.transform.position;
        }
        else {
            _goalHandheldMapRot = (_isFullMap) ? _FullmapPos.transform.rotation : _HandheldPos.transform.rotation;
            _goalHandheldMapPos = (_isFullMap) ? _FullmapPos.transform.position : _HandheldPos.transform.position;
        }

        // lerp position of map camera.
        _MapCamera.transform.position = Vector3.Lerp(_MapCamera.transform.position, _goalMapCameraPos, Time.deltaTime * 10);

        // lerp rotation of handhelds.
        _HandheldContainer.transform.rotation = Quaternion.Lerp(
            _HandheldContainer.transform.rotation,
            _goalHandheldMapRot,
            Time.deltaTime * _smoothHandheldPosition
        );

        // lerp position of handhelds.
        _HandheldContainer.transform.position = Vector3.Lerp(
            _HandheldContainer.transform.position,
            _goalHandheldMapPos,
            Time.deltaTime * _smoothHandheldPosition * 0.5f
        );

        // set map camera zoom.
        _MapCamera.GetComponent<Camera>().orthographicSize = (_isFullMap) ? _mapSizeFull : _mapSizeMini;
        
        //////////////////////
        // only if in fullmap mode
        if(_isFullMap) {
            // move map camera based on player input.
            _goalMapCameraPos += new Vector3( (Input.GetAxis("Horizontal")), 0, (Input.GetAxis("Vertical")) );
        }
        else {
            // map camera follows player.
            _goalMapCameraPos = new Vector3(
                S_Player.Explorer.transform.position.x,
                _MapCamera.transform.position.y,
                S_Player.Explorer.transform.position.z
            );
        }
    }
    #endregion



    #region [Methods]
    public Vector3 MapToWorldPos(Vector2 pos) {
        return _MapCamera.GetComponent<Camera>().ViewportToWorldPoint(pos);
    }

    public void centerCameraOnQuest(Quest quest) {
        var b = new Bounds(_MapCamera.transform.position, Vector2.zero);
        b.Encapsulate(quest.StartPoint.gameObject.GetComponent<Renderer>().bounds);
        b.Encapsulate(quest.EndPoint.gameObject.GetComponent<Renderer>().bounds);
        _goalMapCameraPos = b.center;
    }

    public void placeRedVertex(Vector3 pos) {
        var correctedPos = new Vector3(pos.x, 0, pos.z);
        _ActiveRedline.Add(LineVertex.SpawnVertex(correctedPos, Quaternion.identity, true));
        connectLastRedVertex();
    }

    private void connectLastRedVertex() {
        if(_ActiveRedline.Count > 1) LineVertex.ConnectVertices(_ActiveRedline[_ActiveRedline.Count - 2], LastRedlineVertex);
    }

    public void MoveLatestVertex(Vector3 pos) {
        var correctedPos = new Vector3(pos.x, 0, pos.z);
        LastRedlineVertex.Move(correctedPos);
    }

    public void CancelRedLine() {
        foreach(LineVertex v in _ActiveRedline) {
            Destroy(v.gameObject);
        }
        _ActiveRedline = new List<LineVertex>();
    }

    public void StartNewRedline() {
        CancelRedLine();
        if(Quest.Active == null) throw new Exception("Can't start a redline, not in an active quest!");
        placeRedVertex(Quest.Active.transform.position);
    }
    #endregion



    public static S_Map instance { get; private set; }
    public bool isReady { get; private set;}
}
