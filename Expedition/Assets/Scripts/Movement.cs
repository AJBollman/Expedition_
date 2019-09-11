

// This class controls WASD movement and jumping.
// Put it on the player prefab.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 7f;
    private float verticalVelocity;
    private float gravity = 14f;

    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivity = 5.0f;
    public float smoothing = .20f;
    public string sprintKey = "left shift";
    public string jumpKey = "space";

    private CharacterController controller;
    private Rigidbody chController;



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        chController = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var characterRot = Camera.main.transform.rotation.eulerAngles.y;
        transform.localRotation = Quaternion.AngleAxis(characterRot, Vector3.up);
        float deltaX = Input.GetAxis("Horizontal") * speed;
        float deltaZ = Input.GetAxis("Vertical") * speed;

        Vector3 movement = new Vector3(deltaX, 0, deltaZ);
        movement = Vector3.ClampMagnitude(movement, speed);

        if (controller.isGrounded)
        {
            verticalVelocity = -gravity * Time.deltaTime;
            if (Input.GetKeyDown(jumpKey))
            {
                verticalVelocity = jumpForce;
            }
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }

        if (Input.GetKeyDown(sprintKey)) {speed = 8f;}
        else
        {
            speed = 6f;
        }

        movement.y = verticalVelocity;

        //var desiredMoveDirection =  * deltaX + Camera.main.transform.forward * deltaZ;
        //desiredMoveDirection += (transform.up * verticalVelocity);

        controller.Move(transform.TransformDirection(movement) * Time.deltaTime);
    }
}
