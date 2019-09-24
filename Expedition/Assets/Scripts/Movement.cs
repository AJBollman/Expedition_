

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
    
    private Vector3 slopeAngle;
    public Vector3 slideDirection;



    public bool moveAllowed = true;

    //sphere casting stuff
    public float sphereRadius;
    public float maxDistance;
    public LayerMask layerMask;
    public GameObject currentHitObject;
    private float currentHitDistance;

    private Vector3 orgin;
    private Vector3 direction;

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



        // ********************************* POSITION
        float deltaX = 0f, deltaZ = 0f, speed = 0f;
        if (moveAllowed)
        {
            // Sprint boost.
            if (Input.GetKey(KeyCode.LeftShift)){ speed = sprintSpeed;}
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
            // Apply jump force on [jumpKey].
            if (Input.GetKeyDown(jumpKey))
            {
                verticalVelocity = jumpForce;
            }
        }
        else verticalVelocity -= gravity * Time.deltaTime;

        //=============jumping on slope===============
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, controller.height / 2 * 1.5f))
        {
            if (hit.normal != Vector3.up)
            {
                slopeAngle = hit.normal;
                if (Input.GetKeyDown(jumpKey))
                {
                    verticalVelocity = jumpForce;
                }
                else verticalVelocity -= gravity * Time.deltaTime;
            }
        }


        // Apply movement vectors.
        movement.y = verticalVelocity;
        controller.Move(transform.TransformDirection(movement) * Time.deltaTime);
        
    }
    private void slopeCheck()
    {
        //========slope sliding===========
        orgin = transform.position;
        direction = Vector3.down;

        RaycastHit hit;
        if(Physics.SphereCast(orgin, sphereRadius, direction, out hit, maxDistance, layerMask, QueryTriggerInteraction.UseGlobal))
        {
            currentHitObject = hit.transform.gameObject;
            currentHitDistance = hit.distance;
        }
        else
        {
            currentHitDistance = maxDistance;
            currentHitObject = null;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(orgin, orgin + direction * currentHitDistance);
        Gizmos.DrawWireSphere(orgin + direction * currentHitDistance, sphereRadius);
    }
}