using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItems : MonoBehaviour
{


    public float speed = 2;
    public bool canHold = true;
    private Vector3 smallScale;
    private Vector3 currentScale;
    public GameObject ball;
    public Transform guide;

    private void Awake()
    {
        ball = null;
    }
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!canHold)
                throw_drop();
            else
            {
                if (Physics.Raycast(ray, out hit, 3f))
                {
                    if (hit.transform.gameObject.tag == "Moveable")
                    {
                        ball = hit.transform.gameObject;
                        Pickup();
                    }
                }
            }
        }
    }//update


    void Pickup()
    {
        //scales down the object when you pick it up and grabs the current size
        currentScale = ball.transform.localScale;
        smallScale = currentScale * 0.2f;

        //We set the object parent to our guide empty object.
        ball.transform.SetParent(guide);

        //Set gravity to false while holding it
        ball.GetComponent<Rigidbody>().useGravity = false;

        //we apply the same rotation our main object (Camera) has.
        ball.transform.localRotation = transform.rotation;
        //We re-position the ball on our guide object 
        ball.transform.position = guide.position;
        ball.transform.rotation = guide.rotation;
        ball.transform.localScale = smallScale;
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;


        canHold = false;
    }

    void throw_drop()
    {
        if (!ball)
            return;

        ball.transform.localScale = currentScale;
        //Set our Gravity to true again.
        ball.GetComponent<Rigidbody>().useGravity = true;
        ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        // we don't have anything to do with our ball field anymore
        ball = null;
        //Apply velocity on throwing
        guide.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;

        //Unparent our ball
        guide.GetChild(0).parent = null;
        canHold = true;
    }
}