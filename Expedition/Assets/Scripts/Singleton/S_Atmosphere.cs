
using System;
using UnityEngine;


/// <summary> First-person camera controls. </summary>
[DisallowMultipleComponent]
public sealed class S_Atmosphere : MonoBehaviour
{
    public Color32 GoalSunColor;
    public GameObject _Clouds;
    private Light _Sunlight;

    #region [Events]
    private void OnEnable()
    {
        instance = this;
        try {
            _Clouds.SetActive(true);
            _Sunlight = GetComponentInChildren<Light>();
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
        _Sunlight.color = Color32.Lerp(_Sunlight.color, GoalSunColor, Time.deltaTime * Expedition.smoothLightTransition);
    }
    #endregion

    public static S_Atmosphere instance { get; private set; }
    public bool isReady { get; private set;}
}
