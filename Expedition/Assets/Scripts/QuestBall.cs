using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SoundPlayer))]
[RequireComponent(typeof(SphereCollider))]
public class QuestBall : MonoBehaviour
{
    #region [Private]
    [SerializeField] private bool _isEndpoint;
    private Quest _Owner;
    #endregion

    #region [Public]
    [HideInInspector] public SoundPlayer _SoundPlayer;
    [HideInInspector] public GameObject _MinimapIconObj;
    [HideInInspector] public GameObject _mapIconBorderObj;
    [HideInInspector] public GameObject _BallEffectObj;
    #endregion




    #region [Events]
    void Awake()
    {
        var DebugDraw = transform.Find("TempDebugDrawOffset").gameObject;
        if(DebugDraw) Destroy(DebugDraw);

        _Owner = transform.parent.gameObject.GetComponent<Quest>();
        _SoundPlayer = GetComponent<SoundPlayer>();
        _MinimapIconObj = transform.Find("MapIcon").gameObject;
        _mapIconBorderObj = _MinimapIconObj.transform.Find("MapIconBackground").gameObject;
        _BallEffectObj = transform.Find("BallEffect").gameObject;

        if(!(_Owner || _SoundPlayer || _MinimapIconObj || _mapIconBorderObj || _BallEffectObj) ) {
            enabled = false;
            throw new Exception("QuestBall has missing references!");
        }

        _BallEffectObj.GetComponent<Renderer>().enabled = true;
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player") {
            if(_isEndpoint) _Owner.onEndpointEnter();
            else _Owner.onStartpointEnter();
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.tag == "Player") {
            if(_isEndpoint) _Owner.onEndpointExit();
            else _Owner.onStartpointExit();
        }
    }
    #endregion
}
