

// This class controls WASD movement and jumping.
// Put it on the player prefab.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movementSpeed = 6f;
    public float sprintSpeed = 15f;
    public float jumpForce = 7f;
    public float gravity = 14f;
    public string sprintKey = "left shift";
    public string jumpKey = "space";
    public bool canMove = true;

    private float verticalVelocity;
    private CharacterController controller;
    private Rigidbody controllerRigidbody;
    private GameObject cam;
    
    public float slopeAngle;
    public bool isjumping;

    public bool moveAllowed = true;
    

    private Vector2 playerPOS;
    
    private Vector3 orgin;
    private Vector3 direction;

    private Vector3 jumpPoint;
    private Vector3 slopeHit;
    public Vector3 slopeParallel;
    private Vector3 slopeSlide;
    private Vector3 lerpVector;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controllerRigidbody = GetComponent<Rigidbody>();
        cam = GameObject.Find("CameraContainer");
        if (!cam) throw new System.Exception("Movement could not find a 'CameraContainer'. Make sure one is placed.");
    }

    void Update()
    {
        // ********************************* ROTATION
        // Set the player's Y rotation to match the main camera's.
        float goalAngle = cam.transform.localRotation.eulerAngles.y;
        Vector3 pRot = transform.rotation.eulerAngles;
        pRot.y = goalAngle;
        transform.rotation = Quaternion.Euler(pRot);

        //checking to see if character is allowed to move
        if(moveAllowed == true)
        {
            controllerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else controllerRigidbody.constraints = RigidbodyConstraints.FreezePosition;

        //slopeCheck();

        // ********************************* POSITION
        float deltaX = 0f, deltaZ = 0f, speed = 0f;
        if (moveAllowed)
        {
            // Sprint boost.
            if (Input.GetKeyDown(KeyCode.LeftShift)){ speed = sprintSpeed;}
            else speed = movementSpeed;

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
            verticalVelocity =- gravity * Time.deltaTime;
            isjumping = false;
            // Apply jump force on [jumpKey].
            if (Input.GetKeyDown(jumpKey))
            {
                verticalVelocity = jumpForce;
                isjumping = true;
            }
            else isjumping = false;
        }
        else verticalVelocity -= gravity * Time.deltaTime;

        //=============jumping on slope===============
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * 1.5f))
        {
            if(isjumping == true)
            {
                jumpPoint = hit.point;
            }
            if (hit.normal != Vector3.up)
            {
                //slopeAngle = hit.normal;
                if (Input.GetKeyDown(jumpKey))
                {
                    verticalVelocity = jumpForce;
                    isjumping = true;
                }
                else verticalVelocity -= gravity * Time.deltaTime;
            }
        }
        /*slopeSlide = Vector3.ProjectOnPlane(Vector3.down, slopeHit);
       // movement.y = verticalVelocity;
        movement = transform.TransformDirection(movement);
        lerpVector = Vector3.Lerp(movement, slopeSlide, Mathf.Clamp(slopeAngle / 45f, 0, 1));
        lerpVector.y = verticalVelocity;
        controller.Move(lerpVector * Time.deltaTime);*/
        if(slopeAngle >= controller.slopeLimit && controller.isGrounded)
        {
            //slides the character
            slopeSlide = Vector3.ProjectOnPlane(Vector3.down, slopeHit);
            controller.Move(slopeSlide * (10 * Time.deltaTime));

        }
        else
        {
            // Apply movement vectors.
            movement.y = verticalVelocity;
            controller.Move(transform.TransformDirection(movement) * Time.deltaTime);
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        slopeAngle = Mathf.Round(Vector3.Angle(Vector3.up, hit.normal));
        slopeHit = hit.normal;
        Vector3 groundParallel = Vector3.Cross(transform.up, slopeHit);
        slopeParallel = Vector3.Cross(groundParallel, slopeHit);
    }
}