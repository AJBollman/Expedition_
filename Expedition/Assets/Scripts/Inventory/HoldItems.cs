using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItems : MonoBehaviour
{


    public float speed = 5;
    public bool canHold = true;
    private Vector3 smallScale;
    private Vector3 currentScale;
    public GameObject obj;
    public Transform guide;
    public GameObject startWithObj;

    private void Awake()
    {
        obj = null;
    }

    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.E))
        {
            interact();
        }
    }//update


    private void interact()
    {
        if (!canHold)
            Throw();
        else
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector2(0.5f, 0.5f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 3f))
            {
                if (hit.transform.gameObject.tag == "Moveable")
                {
                    obj = hit.transform.gameObject;
                    canHold = false;
                    Pickup();
                    GetComponent<SoundPlayer>().Play("GrabPlank");
                }
                if (hit.transform.gameObject.tag == "Event")
                {
                    canHold = true;
                    GetComponent<SoundPlayer>().Play("GrabPlank");
                }
            }
        }
    }

    void Pickup()
    {
        //scales down the object when you pick it up and grabs the current size
        currentScale = obj.transform.localScale;
        smallScale = currentScale * 0.2f;

        //We set the object parent to our guide empty object.
        obj.transform.SetParent(guide);

        //Set gravity to false while holding it
        obj.GetComponent<Rigidbody>().useGravity = false;

        //we apply the same rotation our main object (Camera) has.
        obj.transform.localRotation = transform.rotation;
        //We re-position the ball on our guide object 
        obj.transform.position = guide.position;
        obj.transform.rotation = guide.rotation;
        obj.transform.localScale = smallScale;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        //GetComponent<SoundPlayer>().Play("GrabPlank", true);
    }

    void Throw()
    {
        if (!obj)
            return;

        obj.transform.localScale = currentScale;
        //Set our Gravity to true again.
        obj.GetComponent<Rigidbody>().useGravity = true;
        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        // we don't have anything to do with our ball field anymore
        obj = null;
        //Apply velocity on throwing
        guide.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;

        //Unparent our ball
        if(guide.GetChild(0) != null)
        {
            guide.GetChild(0).parent = null;
        }
        canHold = true;
    }
}