
using System;
using UnityEngine;


/// <summary> First-person camera controls. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class S_CameraOperator : MonoBehaviour
{

    #region [Public]
    /// <summary> Set wether rotation is changed. </summary>
    private bool _allowInput;
    public bool AllowInput {
        get => _allowInput;
        set {
            _allowInput = value;
            Cursor.visible = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    /// <summary> Set the object that the camera rotates around. </summary>
    private GameObject _ObjectToFollow;
    public GameObject ObjectToFollow {
        get => _ObjectToFollow;
        set {
            if(value != null) _ObjectToFollow = value;
            else throw new Exception("Camera follow point can not be set to null!");
        }
    }

    /// <summary> Set the object that the camera looks at. </summary>
    [SerializeField] public GameObject ObjectToLookAt { get; set; }

    /// <summary> The camera's field of view. </summary>
    private float _FOV;
    public float FOV {
        get => _FOV;
        set { 
            if(value >= 0.01f && value < 180) _FOV = value; 
        }
    }

    /// <summary> Set wether the Camera looks at the look target.</summary>
    private bool _doLookAtObject;
    public bool DoLookAtObject { 
        get => _doLookAtObject; 
        set => _doLookAtObject = value;
    }

    public float OverrideCameraSmooth { 
        get => _smoothCameraPosition; 
        set => _smoothCameraPosition = value;
    }

    public Camera StableCamera { get; private set; }
    [SerializeField] public float defaultFOV { get; private set; } = 90f;
    #endregion



    #region [Private]
    #if UNITY_WEBGL
    [SerializeField] public float sensitivity = 2f;
    #else
    [SerializeField] public float sensitivity = 10f;
    #endif
    [SerializeField] private float _maxFOVTweak = 7f;
    [SerializeField] private float _maxLookUpAngle = 80f;
    [SerializeField] private float _maxLookDownAngle = -85f;
    [SerializeField] private float _smoothCameraPosition = 64f;
    [SerializeField] private float _smoothCameraShake = 6f;
    [SerializeField] private float _FOVWhileSprinting = 10f;
    [SerializeField] private float _smoothCamRotation = 40f;
    private Vector2 rotDelta;
    private Vector3 _lastPos;
    private Quaternion _goalRot;
    private Animator _Animator;
    private Camera _Camera;
    private GameObject _MainCameraContainer;
    private float _goalFOV;
    #endregion
    


    //////////////////////////////////////////////////////////////////////////////////////////////////
    #region [Events]
    private void OnEnable()
    {
        instance = this;
        try {
            FOV = defaultFOV;
            _Animator = Camera.main.GetComponent<Animator>();
            _MainCameraContainer = transform.Find("Main Camera Container").gameObject;
            StableCamera = _MainCameraContainer.transform.Find("Stable Camera").GetComponent<Camera>();

            if(_Animator == null) throw new Exception("CameraOperator has no animator");
            if(_MainCameraContainer == null) throw new Exception("CameraOperator has no main camera container");
            if(StableCamera == null) throw new Exception("CameraOperator has no stable camera");
            isReady = true;
        }
        catch(Exception e) {
            enabled = false;
            isReady = false;
            Debug.LogException(e);
        }
    }

    private void Update()
    {
        // Apply camera shake based on camera's velocity, biased towards downward vertical velocity, and only if running at higher than 30 FPS.
        _Animator.SetFloat("Shake Amount", 
            Mathf.Lerp(
                _Animator.GetFloat("Shake Amount"),
                (Time.deltaTime < 0.05f ?
                (Vector3.Distance(transform.position, _lastPos)) * ((!Expedition.Movement.isGrounded && transform.position.y < _lastPos.y) ? 2.1f : 0.75f) : 0),
                Time.deltaTime * _smoothCameraShake
            )
        );

        // Lerp container position.
        _lastPos = transform.position;
        transform.position = Vector3.Lerp(
                transform.position,
                ObjectToFollow.transform.position,
                Time.deltaTime * _smoothCameraPosition * Mathf.Clamp(Vector3.Distance(transform.position, ObjectToFollow.transform.position), 0f, 3) * (((Input.GetAxis("Vertical") > 0f || Input.GetAxis("Horizontal") > 0f)) ? 16 : 1)
        );

        // ************** ROTATION
        if (!_doLookAtObject && _allowInput)
        {
            // Set the rotation vectors based on input strength and sensitivity.
            rotDelta.x += Mathf.Clamp(Input.GetAxis("Mouse X") / 10, -1, 1) * sensitivity * 6;
            rotDelta.y += Mathf.Clamp(Input.GetAxis("Mouse Y") / 10, -1, 1) * sensitivity * 6;

            // Limit vertical angle.
            rotDelta.y = Mathf.Clamp(rotDelta.y, _maxLookDownAngle, _maxLookUpAngle);

            // Slight widening of FOV for high/low angles.
            _goalFOV = defaultFOV + Mathf.Clamp(((Mathf.Pow(Mathf.Abs(rotDelta.y), 2)) / 200) - 8, 0, _maxFOVTweak);

            // Lerp camera rotation. DO THE THING!
            _goalRot = Quaternion.Euler(-rotDelta.y, rotDelta.x, 0);
            transform.rotation = _goalRot;/*Quaternion.Lerp(
                transform.rotation,
                _goalRot,
                Time.deltaTime * _smoothCamRotation
            );*/
        }
        else if(_doLookAtObject)
        {
            // Look at object.
            transform.LookAt(ObjectToLookAt.transform.position);

            // TODO zoom in FOV based on how far away it is.
            _goalFOV = FOV + 10f;
            Mathf.Clamp(_goalFOV, 15f, FOV + 10f);
        }
        
        // Lerp FOV.
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _goalFOV + 
            ((Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0.1f) ? _FOVWhileSprinting : 0),
            Time.deltaTime * 8
        );

    }
    #endregion



    public static S_CameraOperator instance { get; private set; }
    public bool isReady { get; private set;}
}