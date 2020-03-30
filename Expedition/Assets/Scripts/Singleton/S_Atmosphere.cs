
using System;
using UnityEngine;


/// <summary> First-person camera controls. </summary>
[DisallowMultipleComponent]
public sealed class S_Atmosphere : MonoBehaviour
{
    public GameObject _Clouds;

    #region [Events]
    private void OnEnable()
    {
        instance = this;
        try {
            _Clouds.SetActive(true);
            isReady = true;
        }
        catch(Exception e) {
            enabled = false;
            isReady = false;
            Debug.LogException(e);
        }
    }

    void Update()
    {
        _Clouds.transform.position = Camera.main.transform.position;
    }
    #endregion

    public static S_Atmosphere instance { get; private set; }
    public bool isReady { get; private set;}
}
