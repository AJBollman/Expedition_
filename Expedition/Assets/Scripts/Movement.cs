
using UnityEngine;

/// <summary> This class controls WASD movement and jumping. Boing </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public sealed class Movement : MonoBehaviour
{
    /////////////////////////////////////////////////   Public properties
    private static bool _isjumping;
    public static bool isJumping {
        get => _isjumping;
    }
    private static bool _isSprinting;
    public static bool isSprinting {
        get => _isSprinting;
    }
    public static bool isGrounded {
        get => controller.isGrounded;
    }
    /// <summary> Set wether the character can move. Gravity is still applied.static </summary>
    public static bool moveAllowed {
        get => _moveAllowed;
        set => _moveAllowed = value;
    }
    [SerializeField] private static bool _moveAllowed = true;

    

    /////////////////////////////////////////////////   Private, Serializable fields
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float gravity = 14f;
    [SerializeField] private string jumpKey = "space";



    /////////////////////////////////////////////////   Private fields
    private float speed = 0f;
    private static CharacterController controller;
    private bool jumpSprintSpeed;
    private float verticalVelocity;
    private Rigidbody controllerRigidbody;
    private Vector3 slopeHit;
    private float timer;
    private bool stuck;
    private GameObject stableCam;
    private float slopeAngle;


    public static bool isReady { get; private set;}





    //////////////////////////////////////////////////////////////////////////////////////////////////  Events
    void OnEnable()
    {
        controller = GetComponent<CharacterController>();
        controllerRigidbody = GetComponent<Rigidbody>();
        //if (Application.isEditor) sprintSpeed = 25f;
        stableCam = GameObject.Find("Stable Camera");
        if(stableCam == null) {
            enabled = false;
            throw new System.Exception("Movement requires a 'Stable Camera'. Is there a CameraContainer in the scene?");
        }
        isReady = true;
    }

    void Update()
    {
        // ********************************* ROTATION
        // Set the player's Y rotation to match the main camera's.
        float goalAngle = stableCam.transform.rotation.eulerAngles.y;
        Vector3 pRot = transform.rotation.eulerAngles;
        pRot.y = goalAngle;
        transform.rotation = Quaternion.Euler(pRot);

        //checking to see if character is allowed to move
        if (_moveAllowed == true)
        {
            controllerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else controllerRigidbody.constraints = RigidbodyConstraints.FreezePosition;

        // ********************************* POSITION
        float deltaX = 0f, deltaZ = 0f;
        if (_moveAllowed)
        {
            // Sprint boost.
            if (Input.GetKey(KeyCode.LeftShift) && _isjumping == false)
            {
                _isSprinting = true;
                speed = sprintSpeed;
            }
            else
            {
                _isSprinting = false;
                speed = movementSpeed;
            }

            if(jumpSprintSpeed == true)
            {
                speed = sprintSpeed;
                if(_isjumping == false)
                {
                    jumpSprintSpeed = false;
                    speed = movementSpeed;
                }
            }

            // Get forward/backward and side-to-side control inputs.
            deltaX = Input.GetAxis("Horizontal") * speed;
            deltaZ = Input.GetAxis("Vertical") * speed;
        }

        // Create vector from inputs.
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        // Apply gravity.
        if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            _isjumping = false;
            // Apply jump force on [jumpKey].
            if (Input.GetKeyDown(jumpKey))
            {
                if (_isSprinting == true)
                {
                    jumpSprintSpeed = true;
                }
                verticalVelocity = jumpForce;
                _isjumping = true;
            }
            else _isjumping = false;
        }
        else verticalVelocity -= gravity * Time.deltaTime;

        //=============jumping on slope===============
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * 2f))
        {
            /*if (isjumping == true)
            {
                jumpPoint = hit.point;
            }*/
            if (hit.normal != Vector3.up)
            {
                //slopeAngle = hit.normal;
                if (Input.GetKeyDown(jumpKey))
                {
                    verticalVelocity = jumpForce;
                }
                else verticalVelocity -= gravity * Time.deltaTime;
            }
        }

        if (slopeAngle >= controller.slopeLimit && controller.isGrounded && stuck == false)
        {
            //slides the character
            var slopeSlide = Vector3.ProjectOnPlane(Vector3.down, slopeHit);
            controller.Move(slopeSlide * (10 * Time.deltaTime));

            timer += Time.deltaTime;
            //Debug.Log(timer);
            if(timer >= 5){ stuck = true;}
        }
        else
        {
            // Apply movement vectors.
            movement.y = verticalVelocity;
            controller.Move(transform.TransformDirection(movement) * Time.deltaTime);

            if(stuck == true)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    stuck = false;
                }
            }
            else
            {
                timer = 0;
            }
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        slopeAngle = Mathf.Round(Vector3.Angle(Vector3.up, hit.normal));
        slopeHit = hit.normal;
        Vector3 groundParallel = Vector3.Cross(transform.up, slopeHit);
    }
}