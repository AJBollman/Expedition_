using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOperator : MonoBehaviour
{
    // IMPORTANT STUFF
    public bool enableControls = true;
    public GameObject followPoint;
    public float defaultFOV = 80f;
    public Vector3 sensitivity = new Vector3(1f, 0.65f, 1f);
    public float sensMultiplier = 1f;

    // SMALL TWEAKS
    public float maxFOVTweak = 25f;
    public float maxLookUpAngle = 60f;
    public float maxLookDownAngle = -70f;

    // COULD BE USEFUL??
    public bool enableLookAt = false;
    public GameObject lookAt;
    public bool doHorizontalBias = false;


    private float _lastHeight;
    private bool _heightChangeBelowThreshold;

    private Vector3 _pivotPoint = new Vector3(0, 1f, 0);
	private Vector3 _truePivot = 	new Vector3(0,1f,0);
    private Camera _camCamera;
    private float _goalFOV;
    private Vector3 _rotDelta = Vector3.zero;

	private Quaternion _aim;
	private Vector2 _axisDamper = new Vector2(3f, 3f);
    private float _distToTLookTarget = 0f;
    private Vector2 _input = Vector2.zero;


    //********************************************************************************************************
    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _goalFOV = defaultFOV;
		_camCamera = GetComponentInChildren<Camera>();
        followPoint = GameObject.Find("The Explorer");
    }

    public float smoothTime = 8f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            if (sensMultiplier >= 2) { sensMultiplier--; Debug.Log("Decreasing sens to " + sensMultiplier);}
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket)){
            if (sensMultiplier <= 15) {sensMultiplier++; Debug.Log("Increasing sens to " + sensMultiplier);}
        }


        //******* PIVOT and CONTAINER POSITION
        _pivotPoint = followPoint.transform.position;
        _lastHeight = _pivotPoint.y;

        _pivotPoint.x = transform.position.x - followPoint.transform.position.x;
        _pivotPoint.y = transform.position.y - followPoint.transform.position.y;
        _pivotPoint.z = transform.position.z - followPoint.transform.position.z;

        // Set camera 3D position, with height offset.
        _truePivot.x = Mathf.Lerp(_pivotPoint.x, 0f, Time.deltaTime * smoothTime);
        _truePivot.y = Mathf.Lerp(_pivotPoint.y, 0f, Time.deltaTime * smoothTime);
        _truePivot.z = Mathf.Lerp(_pivotPoint.z, 0f, Time.deltaTime * smoothTime);

        transform.position = new Vector3(
            followPoint.transform.position.x + _truePivot.x,
            followPoint.transform.position.y + _truePivot.y,
            followPoint.transform.position.z + _truePivot.z
        );

        // ************** ROTATION
        if (enableControls)
        {
            if (enableLookAt)
            {
                transform.LookAt(lookAt.transform.position);
            }
            else
            {
                // Set inputs.
                _input.x = Input.GetAxis("Mouse X");
                _input.y = Input.GetAxis("Mouse Y");

                if (doHorizontalBias)
                {
                    if (Mathf.Abs(Input.GetAxis("Mouse X")) > Mathf.Abs(Input.GetAxis("Mouse Y"))) _axisDamper.y = 0f;
                    else _axisDamper.y = 3f;
                }

                // Set rotation deltas.
                _rotDelta.x += Mathf.Clamp(_input.x / 10, -1, 1) * sensitivity.x * sensMultiplier * _axisDamper.x * 2;
                _rotDelta.y += Mathf.Clamp(_input.y / 10, -1, 1) * sensitivity.y * sensMultiplier * _axisDamper.y * 2;

                // Limit vertical angle.
                _rotDelta.y = Mathf.Clamp(_rotDelta.y, maxLookDownAngle, maxLookUpAngle);

                // Set camera angle. DO THE THING!
                _aim = Quaternion.Euler(-_rotDelta.y, _rotDelta.x, 0);
                transform.rotation = _aim;
            }
        }


        // ****************************** FIELD-OF-VIEW
        if (enableLookAt)
        {
            _goalFOV = defaultFOV + 10f - _distToTLookTarget/2;
            Mathf.Clamp(_goalFOV, 15f, defaultFOV + 10f);
        }
        // Slight widening of FOV for high/low angles.
        _goalFOV = defaultFOV + Mathf.Clamp(((Mathf.Pow(Mathf.Abs(_rotDelta.y), 2)) / 200) - 8, 0, maxFOVTweak);

        // Lerp to FOV goal.
        _camCamera.fieldOfView = Mathf.Lerp(_camCamera.fieldOfView, _goalFOV, Time.deltaTime * 8);

    }
}