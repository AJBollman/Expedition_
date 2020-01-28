
using System;
using UnityEngine;

/// <summary> This class controls WASD movement and jumping. Boing. </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CharacterController))]
public sealed class S_Movement : MonoBehaviour 
{

    #region [Public]

    public bool IsJumping { get; private set; }

    public bool IsSprinting { get; private set; }

    public bool isGrounded {
        get => _CharacterController.isGrounded;
    }

    /// <summary> Set wether the character can move. Gravity is still applied. </summary>
    private bool _allowInput = true;
    public bool AllowInput {
        get => _allowInput;
        set => _allowInput = value;
    }
    #endregion

    

    #region [Private]
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _sprintSpeed = 9f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _gravity = 18f;
    [SerializeField] private string _jumpKey = "space";
    private static CharacterController _CharacterController;
    private Rigidbody _Rigidbody;
    private Vector3 _slopeHit;
    private float _speed;
    private bool _jumpSprintSpeed;
    private float _verticalVelocity;
    private float _timer;
    private bool _isStuck;
    private float _slopeAngle;
    #endregion



    //////////////////////////////////////////////////////////////////////////////////////////////////
    #region [Events]
    void OnEnable()
    {
        instance = this;
        try {
            _CharacterController = GetComponent<CharacterController>();
            _Rigidbody = GetComponent<Rigidbody>();
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
        // Set the player's Y rotation to match the main camera's.
        float goalAngle = Expedition.CameraOperator.StableCamera.transform.rotation.eulerAngles.y;
        Vector3 pRot = transform.rotation.eulerAngles;
        pRot.y = goalAngle;
        transform.rotation = Quaternion.Euler(pRot);


        // Check to see if character is allowed to move.
        if (_allowInput == true)
        {
            _Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else _Rigidbody.constraints = RigidbodyConstraints.FreezePosition;


        // Set position.
        float deltaX = 0f, deltaZ = 0f;
        if (_allowInput)
        {
            // Sprint boost.
            if (Input.GetKey(KeyCode.LeftShift) && IsJumping == false)
            {
                IsSprinting = true;
                _speed = _sprintSpeed;
            }
            else
            {
                IsSprinting = false;
                _speed = _movementSpeed;
            }

            if(_jumpSprintSpeed == true)
            {
                _speed = _sprintSpeed;
                if(IsJumping == false)
                {
                    _jumpSprintSpeed = false;
                    _speed = _movementSpeed;
                }
            }

            // Get forward/backward and side-to-side control inputs.
            deltaX = Input.GetAxis("Horizontal") * _speed;
            deltaZ = Input.GetAxis("Vertical") * _speed;
        }

        // Create vector from inputs.
        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, _speed);

        // Apply gravity.
        if (_CharacterController.isGrounded)
        {
            _verticalVelocity = -_gravity * Time.deltaTime;
            IsJumping = false;
            // Apply jump force on [jumpKey].
            if (Input.GetKeyDown(_jumpKey))
            {
                if (IsSprinting == true)
                {
                    _jumpSprintSpeed = true;
                }
                _verticalVelocity = _jumpForce;
                IsJumping = true;
            }
            else IsJumping = false;
        }
        else _verticalVelocity -= _gravity * Time.deltaTime;

        //=============jumping on slope===============
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, _CharacterController.height / 2 * 2f))
        {
            /*if (isjumping == true)
            {
                jumpPoint = hit.point;
            }*/
            if (hit.normal != Vector3.up)
            {
                //slopeAngle = hit.normal;
                if (Input.GetKeyDown(_jumpKey))
                {
                    _verticalVelocity = _jumpForce;
                }
                else _verticalVelocity -= _gravity * Time.deltaTime;
            }
        }

        if (_slopeAngle >= _CharacterController.slopeLimit && _CharacterController.isGrounded && _isStuck == false)
        {
            //slides the character
            var slopeSlide = Vector3.ProjectOnPlane(Vector3.down, _slopeHit);
            _CharacterController.Move(slopeSlide * (10 * Time.deltaTime));

            _timer += Time.deltaTime;
            //Debug.Log(timer);
            if(_timer >= 5){ _isStuck = true;}
        }
        else
        {
            // Apply movement vectors.
            movement.y = _verticalVelocity;
            _CharacterController.Move(transform.TransformDirection(movement) * Time.deltaTime);

            if(_isStuck == true)
            {
                _timer -= Time.deltaTime;
                if(_timer <= 0)
                {
                    _isStuck = false;
                }
            }
            else
            {
                _timer = 0;
            }
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _slopeAngle = Mathf.Round(Vector3.Angle(Vector3.up, hit.normal));
        _slopeHit = hit.normal;
        Vector3 groundParallel = Vector3.Cross(transform.up, _slopeHit);
    }
    #endregion



    public static S_Movement instance { get; private set; }
    public bool isReady { get; private set;}
}