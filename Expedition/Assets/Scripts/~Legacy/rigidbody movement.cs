#if false

// This class controls WASD movement and jumping.
// Put it on the player prefab.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float movementSpeed = 6f;
    public float sprintSpeed = 9f;
    public float jumpForce = 7f;
    public float gravity = 14f;
    public float maxMoveSpeed = 2f;
    public string sprintKey = "left shift";
    public string jumpKey = "space";
    public bool canMove = true;

    private float verticalVelocity;
    //private CharacterController controller;
    private GameObject cam;
    private Rigidbody rbody;

    void Start()
    {
        //controller = GetComponent<CharacterController>();
        cam = GameObject.Find("CameraContainer");
        if (!cam) throw new System.Exception("Movement could not find a 'CameraContainer'. Make sure one is placed.");
        rbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //  ROTATION
        // Set the player's Y rotation to match the main camera's.
        float goalAngle = cam.transform.localRotation.eulerAngles.y;
        Vector3 pRot = transform.rotation.eulerAngles;
        pRot.y = goalAngle;
        transform.rotation = Quaternion.Euler(pRot);



        //  POSITION
        float deltaX = 0f, deltaZ = 0f, speed = 0f;
        if (canMove)
        {
            // Sprint boost.
            if (Input.GetKeyDown(sprintKey)) { speed = sprintSpeed; }
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
            // Apply jump force on [jumpKey].
            if (Input.GetKeyDown(jumpKey)) verticalVelocity = jumpForce;
            else verticalVelocity = 0f;
        }
        else verticalVelocity -= gravity * Time.deltaTime;*/

        // Apply movement vectors.
        //movement.y = verticalVelocity;
        //controller.Move(transform.TransformDirection(movement) * Time.deltaTime);

        movement.y = Input.GetKeyDown(jumpKey) ? jumpForce : 0f;
        if (rbody.velocity.magnitude < maxMoveSpeed)
        {
            rbody.AddForce(transform.TransformDirection(movement), ForceMode.Impulse);
        }
        //else rbody.velocity = maxMoveSpeed;
    }
}
#endif