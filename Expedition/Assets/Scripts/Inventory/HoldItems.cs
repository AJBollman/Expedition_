using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItems : MonoBehaviour
{


    public float speed = 5;
    public Transform guide;
    public GameObject startWithObject;
    public Camera cameraOverride;

    private Camera raycastCamera;
    private GameObject heldObject;

    private Transform goal;
    private Vector3 initialScale;
    private Vector3 goalScale;
    private bool isPlacing;

    private void Awake()
    {
        if (startWithObject != null) Pickup(startWithObject);
        if (raycastCamera == null) raycastCamera = Camera.main;
    }

    public GameObject getHeldObject()
    {
        return heldObject;
    }

    private void Update()
    {
        if(heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, goal.position, Time.deltaTime * 15f);
            heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation, goal.rotation, Time.deltaTime * 15f);
            heldObject.transform.localScale = Vector3.Lerp(heldObject.transform.localScale, goalScale, Time.deltaTime * 15f);
        }
    }




    public void Pickup(GameObject pickingUpObject)
    {
        if (pickingUpObject.tag != "Moveable" || isPlacing) return;

        if(pickingUpObject.transform.parent != null && pickingUpObject.transform.parent.tag == "Event")
        {
            pickingUpObject.transform.parent.GetComponent<MeshRenderer>().enabled = true;
        }
        heldObject = pickingUpObject;
        //scales down the object when you pick it up and grabs the current size
        //currentScale = heldObject.transform.localScale;
        //smallScale = currentScale * 0.2f;

        //We set the object parent to our guide empty object.
        initialScale = pickingUpObject.transform.localScale;
        heldObject.transform.SetParent(guide);
        goalScale = initialScale * 0.1f;
        goal = guide.transform;

        //Set gravity to false while holding it
        heldObject.GetComponent<Rigidbody>().useGravity = false;

        //we apply the same rotation our main object (Camera) has.
        /*heldObject.transform.localRotation = transform.rotation;
        //We re-position the ball on our guide object 
        heldObject.transform.position = guide.position;
        heldObject.transform.rotation = guide.rotation;
        heldObject.transform.localScale = smallScale;*/
        heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        heldObject.GetComponent<Collider>().enabled = false;
        SoundPlayer sound = pickingUpObject.GetComponent<SoundPlayer>();
        if(sound != null)
        {
            sound.Play("Grab");
        }
    }




    public void Drop(bool yeet)
    {
        if (heldObject == null || isPlacing) return;

        heldObject.transform.localScale = initialScale;
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if(yeet)
        {
            heldObject.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;
        }

        if (guide.GetChild(0) != null)
        {
            guide.GetChild(0).parent = null;
        }
        StartCoroutine(yeetEnableCollider(0.25f, heldObject.transform));
        heldObject = null;
    }




    public void Place(Transform at)
    {
        if (heldObject == null || isPlacing) return;

        SoundPlayer sound = heldObject.GetComponent<SoundPlayer>();
        if (sound != null)
        {
            sound.Play("Grab");
        }

        isPlacing = true;

        /*goal.localPosition = at.localPosition;
        goal.rotation = at.rotation;
        goal.localScale = at.localScale;*/
        goal = at;
        goal.localPosition += new Vector3(0, 1f, 0);
        goalScale = initialScale;

        StartCoroutine(unparentAndReenable(0.5f, at));
    }


    private IEnumerator yeetEnableCollider(float delay, Transform obj)
    {
        yield return new WaitForSeconds(delay);
        obj.gameObject.GetComponent<Collider>().enabled = true;
    }


    private IEnumerator unparentAndReenable(float delay, Transform srslyunity)
    {
        yield return new WaitForSeconds(delay);
        heldObject.GetComponent<Collider>().enabled = true;
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        //heldObject.transform.SetParent(srslyunity);
        heldObject = null;

        if (guide.GetChild(0) != null)
        {
            guide.GetChild(0).parent = null;
        }
        srslyunity.localPosition += new Vector3(0, -1f, 0);
        srslyunity.GetComponent<MeshRenderer>().enabled = false;
        isPlacing = false;
    }
}