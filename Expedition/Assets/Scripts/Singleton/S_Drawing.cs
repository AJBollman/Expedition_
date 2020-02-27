
using System;
using UnityEngine;

/// <summary> Handles the player's ability to draw. </summary>
[DisallowMultipleComponent]
public sealed class S_Drawing : MonoBehaviour
{

    #region [Public]
    /// <summary> Set wether the character can draw lines. </summary>
    private bool _allowCameraDrawing = true;
    public bool AllowCameraDrawing {
        get => _allowCameraDrawing;
        set => _allowCameraDrawing = value;
    }
    public bool IsDrawing { get => (_LastVertex != null); }
    public bool hasLOS { get; private set; } = true;
    #endregion

    #region [Private]
    [SerializeField] private Gradient _ColorCanDrawLine;
    [SerializeField] private Gradient _ColorCannotDrawLine;
    [SerializeField] private Vector2 _ScreenCoordsRaycastOrigin = new Vector2(0.5f, 0.42f);
    [SerializeField] private float _smoothIndicatorPos = 20f;
    [SerializeField] private float _smoothIndicatorRot = 10f;
    [SerializeField] private float _maxRaycastDistance = 15f;
    [SerializeField] private float _volumeDrawDrone = 0.5f;
    private LineVertex _NewVert;
    private LineVertex _NewRedVert;
    public GameObject _Indicator;
    private GameObject _LastIndicator;
    private GameObject _IndicatorTip;
    private GameObject _LastIndicatorTip;
    private LineVertex _LastVertex;
    public LineVertex _LastRedVertex { get; private set; }
    private Vector3 _goalIndicatorPos;
    private Quaternion _goalIndicatorRot;
    private bool _canPlaceLine;
    #endregion



    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    #region [Events]
    private void OnEnable() {
        instance = this;
        try {
            _Indicator = GameObject.Find("Vertex Indicator");
            _LastIndicator = GameObject.Find("Last Vertex Indicator");
            _IndicatorTip = _Indicator.transform.Find("Tip").gameObject;
            _LastIndicatorTip = _LastIndicator.transform.Find("Tip").gameObject;

            if(_Indicator == null) throw new Exception("Player has no vertex indicator");
            if(_LastIndicator == null) throw new Exception("Player has no last indicator");
            if(_IndicatorTip == null) throw new Exception("indicator has no indicatorEnd");
            if(_LastIndicatorTip == null) throw new Exception("lastindicator has no indicatorEnd");

            _LastVertex = null;
            _LastIndicator.SetActive(false);

            isReady = true;
        }
        catch(Exception e) {
            isReady = false;
            enabled = false;
            Debug.LogException(e);
        }
    }

    private void Start() {
        Expedition.IndicatorLine.positionCount = 2;
    }

    private void Update() {
        // stuff to do regardless:
        _Indicator.SetActive(_allowCameraDrawing);
        _LastIndicator.SetActive(_allowCameraDrawing && _LastVertex != null);


        // stuff to do only if is drawingMode:
        if(_allowCameraDrawing) {

            // try to move the line indicator to where the Player is looking.
            if(Expedition.Player.viewRaycast(_maxRaycastDistance, _ScreenCoordsRaycastOrigin)) {
                _goalIndicatorPos = Expedition.Player.lastRaycastHit.point;
                _goalIndicatorRot = Quaternion.FromToRotation(Vector3.up, Expedition.Player.lastRaycastHit.normal);
            }

            // lerp line indicator position and rotation.
            _Indicator.transform.position = Vector3.Lerp(_Indicator.transform.position, _goalIndicatorPos, Time.deltaTime * _smoothIndicatorPos);
            _Indicator.transform.rotation = Quaternion.Slerp(_Indicator.transform.rotation, _goalIndicatorRot, Time.deltaTime * _smoothIndicatorRot);

            // draw line between 
            if(_LastVertex != null) {
                Expedition.IndicatorLine.positionCount = 2;
                Expedition.IndicatorLine.SetPosition(0, _LastIndicatorTip.transform.position);
                Expedition.IndicatorLine.SetPosition(1, _IndicatorTip.transform.position);
            }
            else {
                Expedition.IndicatorLine.positionCount = 0;
            }

            // check if the indicator has line-of-sight to the previous indicator (assuming there is a previous one).
            if(_LastVertex != null) {
                hasLOS = !Physics.Linecast(_IndicatorTip.transform.position, _LastIndicatorTip.transform.position, layerMask:Expedition.raycastIgnoreLayers);
                _canPlaceLine = hasLOS;
                Expedition.IndicatorLine.colorGradient = (hasLOS) ? _ColorCanDrawLine : _ColorCannotDrawLine;
                Expedition.DrawDroneClear.volume = (hasLOS) ? _volumeDrawDrone : 0;
                Expedition.DrawDroneUnclear.volume = (hasLOS) ? 0 : _volumeDrawDrone;
            }
            else {
                _canPlaceLine = false;
                Expedition.DrawDroneClear.volume = 0;
                Expedition.DrawDroneUnclear.volume = 0;
            }


        } // end if allowCameraDrawing
    }
    #endregion



    #region [Methods]
    public void cancelLine() {
        _LastIndicator.SetActive(false);
        _LastVertex = null;
        hasLOS = true;
    }

    public void placeVertex() {
        if(!_allowCameraDrawing) return; // TODO give some feedback to the player
        _NewVert = LineVertex.SpawnVertex(_goalIndicatorPos, _goalIndicatorRot);
        _LastIndicator.transform.position = _Indicator.transform.position;
        _LastIndicator.transform.rotation = _Indicator.transform.rotation;
    }

    public void connectVertex() {
        if(!_allowCameraDrawing) return; // TODO give some feedback to the player
        if(_LastVertex != null) LineVertex.ConnectVertices(_LastVertex, _NewVert);
        _LastVertex = _NewVert;
    }

    #endregion



    public static S_Drawing instance { get; private set; }
    public bool isReady { get; private set;}
}
