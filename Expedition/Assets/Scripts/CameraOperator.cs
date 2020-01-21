
using UnityEngine;

/// <summary> First-person camera controls. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class CameraOperator : MonoBehaviour
{
    //////// TESTING
    public float shakeSmooth;
    public float sprintExpandFOV;
    public float handheldSmooth;
    public float lookSmooth;
    public static bool doLookAtObject;


    /////////////////////////////////////////////////   Public properties
    /// <summary> Set wether rotation is changed. </summary>
    private bool _enableControls;
    public static bool enableControls {
        get => _inst._enableControls;
        set {
            _inst._enableControls = value;
            Cursor.visible = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            //Debug.LogWarning("Toggled camera controls");
        }
    } 
    /// <summary> Set the object that the camera rotates around. </summary>
    [SerializeField] private GameObject _followObject;
    public static GameObject followObject {
        get => _inst._followObject;
        set {
            if(value != null) _inst._followObject = value;
            else throw new System.Exception("Camera follow point can not be set to null!");
        }
    }
    /// <summary> Set the object that the camera looks at. </summary>
    [SerializeField] private GameObject _lookAtObject;
    public static GameObject lookAtObject {
        get => _inst._lookAtObject;
        set => _inst._lookAtObject = value;
    }
    /// <summary> Set the camera's field of view. </summary>
    private float _FOV = 90f;
    public static float FOV {
        get => _inst._FOV;
        set { if(value >= 0.01f && value < 180) _inst._FOV = value; }
    }
    public static bool isReady { get; private set;}


    /////////////////////////////////////////////////   Private, Serializable fields
    [SerializeField][Range(0.01f, 180f)] private float defaultFOV = 90f;
    [SerializeField] private float sensitivity = 10f;
    [SerializeField] private float maxFOVTweak = 25f;
    [SerializeField] private float maxLookUpAngle = 60f;
    [SerializeField] private float maxLookDownAngle = -70f;
    [SerializeField] private float smoothTime = 8f;



    /////////////////////////////////////////////////  Private fields
    private static Animator camShake;
    private static Vector3 lastPos;
    private static float _goalFOV; // what FOV is being lerp'd to.
    private static Quaternion _goalRot;
    private static Vector3 _rotDelta = Vector3.zero; // this vector is calculated from mouse input every frame
    private static GameObject mainCamContainer;
    private static GameObject handheldContainer;
    private static GameObject handheldGoal;


    // Singleton instance
    private static CameraOperator _inst;
    /// <summary> The CameraContainer game object. </summary>
    public static GameObject Object {get => _inst.gameObject;}





    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    private void Awake() {_inst = this;}

    private void OnEnable()
    {
        enableControls = false;
        FOV = defaultFOV;
        //if(followObject == null) Debug.LogWarning("Camera has no gameObject to follow");
        camShake = Camera.main.GetComponent<Animator>();
        if(camShake == null) Debug.LogWarning("Main camera has no animator, camera shake will not work");
        mainCamContainer = GameObject.Find("Main Cam Container");
        handheldContainer = GameObject.Find("Handheld Container");
        handheldGoal = GameObject.Find("Handheld Goal");
        if(mainCamContainer == null) {
            enabled = false;
            throw new System.Exception("CameraOperator is missing 'Main Cam Container' object");
        }
        if(handheldContainer == null) {
            enabled = false;
            throw new System.Exception("CameraOperator is missing 'Handheld Container' object");
        }
        if(handheldGoal == null) {
            enabled = false;
            throw new System.Exception("CameraOperator is missing 'Handheld Goal' object");
        }
        isReady = true;
    }

    private void Update()
    {
        /*if(Time.deltaTime > 0.03f && Camera.main.farClipPlane > 500) Camera.main.farClipPlane--;
        else if (Camera.main.farClipPlane < 1000) Camera.main.farClipPlane++;*/

        // Apply camera shake based on camera's velocity, biased towards downward vertical velocity.
        camShake.SetFloat("Shake Amount", 
            Mathf.Lerp(
                camShake.GetFloat("Shake Amount"),
                (Time.deltaTime < 0.05f ?
                (Vector3.Distance(transform.position, lastPos)) * ((!Movement.isGrounded && transform.position.y < lastPos.y) ? 2.1f : 0.75f) : 0),
                Time.deltaTime * shakeSmooth
            )
        );
        /*if (Input.GetKeyDown(KeyCode.LeftBracket)) {
            if (sensitivity >= 2) { sensitivity--; Debug.Log("Decreasing sens to " + sensitivity);}
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket)){
            if (sensitivity <= 15) {sensitivity++; Debug.Log("Increasing sens to " + sensitivity);}
        }*/

        //******* PIVOT and CONTAINER POSITION
        lastPos = transform.position;
            transform.position = Vector3.Lerp(
                transform.position,
                followObject.transform.position,
                Time.deltaTime * smoothTime * Mathf.Clamp(Vector3.Distance(transform.position, followObject.transform.position), 0f, 3) * (((Input.GetAxis("Vertical") > 0f || Input.GetAxis("Horizontal") > 0f)) ? 16 : 1)
        );

        // ************** ROTATION
        if (!doLookAtObject && enableControls)
        {
            // Set rotation deltas.
            _rotDelta.x += Mathf.Clamp(Input.GetAxis("Mouse X") / 10, -1, 1) * sensitivity * 6;
            _rotDelta.y += Mathf.Clamp(Input.GetAxis("Mouse Y") / 10, -1, 1) * sensitivity * 6;

            // Limit vertical angle.
            _rotDelta.y = Mathf.Clamp(_rotDelta.y, maxLookDownAngle, maxLookUpAngle);

            // Slight widening of FOV for high/low angles.
            _goalFOV = defaultFOV + Mathf.Clamp(((Mathf.Pow(Mathf.Abs(_rotDelta.y), 2)) / 200) - 8, 0, maxFOVTweak);

            // Set camera angle. DO THE THING!
            _goalRot = Quaternion.Euler(-_rotDelta.y, _rotDelta.x, 0);
            mainCamContainer.transform.rotation = Quaternion.Lerp(
            mainCamContainer.transform.rotation,
            _goalRot,
            Time.deltaTime * lookSmooth
        );
        }
        else if(doLookAtObject)
        {
            // Look at object
            mainCamContainer.transform.LookAt(lookAtObject.transform.position);

            // zoom in FOV based on how far away it is. TODO
            _goalFOV = defaultFOV + 10f;
            Mathf.Clamp(_goalFOV, 15f, defaultFOV + 10f);
        }

        // Follow-through / delayed motion for hand-held objects
        handheldContainer.transform.rotation = Quaternion.Lerp(
            handheldContainer.transform.rotation,
            handheldGoal.transform.rotation,
            Time.deltaTime * handheldSmooth
        );
        handheldContainer.transform.position = Vector3.Lerp(
            handheldContainer.transform.position,
            handheldGoal.transform.position,
            Time.deltaTime * handheldSmooth * 0.5f
        );
        
        // Lerp to FOV goal.
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, _goalFOV + 
            ((Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0.1f) ? sprintExpandFOV : 0),
            Time.deltaTime * 8
        );

    }
}