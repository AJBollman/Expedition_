using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItems : MonoBehaviour
{
    //inventory sprites and inventory
    private Inventory inventory;
    public GameObject PlankImg;

    public float speed = 5;
    public Transform guide;
    public GameObject startWithObject;

    private Camera raycastCamera;
    private GameObject heldObject;

    private Transform goal;
    private Vector3 initialScale;
    private Vector3 goalScale;
    private bool isPlacing;
    private GameObject checkOnDropped;

    private void Awake()
    {
        if (raycastCamera == null) raycastCamera = Camera.main;
    }

    public GameObject getHeldObject()
    {
        return heldObject;
    }

    public GameObject getCheckOnDropped()
    {
        return checkOnDropped;
    }

    private void Update()
    {
        if (heldObject != null)
        {
            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, goal.position, Time.deltaTime * 15f);
            heldObject.transform.rotation = Quaternion.Lerp(heldObject.transform.rotation, goal.rotation, Time.deltaTime * 15f);
            heldObject.transform.localScale = Vector3.Lerp(heldObject.transform.localScale, goalScale, Time.deltaTime * 15f);
        }
    }

    /*private void FixedUpdate()
    {
        if(checkOnDropped != null &&
            heldObject == null &&
            transform.position.y - checkOnDropped.transform.position.y > 3f &&
            Vector3.Distance(gameObject.transform.position, checkOnDropped.transform.position) > 3f
        )
        {
            Pickup(checkOnDropped);
        }
    }*/

    public void Start()
    {
        if (startWithObject != null) Pickup(startWithObject.gameObject);
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        if(PlankImg) PlankImg.active = false;
    }




    public void Pickup(GameObject pickingUpObject)
    {
        if (pickingUpObject.tag != "Moveable" || isPlacing || heldObject != null) return;

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

        if(PlankImg) PlankImg.active = true;
        checkOnDropped = null;
        heldObject.GetComponent<Renderer>().material.renderQueue = 0;
    }




    public void Drop(bool yeet)
    {
        //Debug.Log("akhsvd");
        if (heldObject == null || isPlacing) return;

        //goal.localPosition += new Vector3(0, 1f, 0);
        if (guide.GetChild(0) != null)
        {
            guide.GetChild(0).parent = null;
        }

        heldObject.transform.localPosition = goal.position + new Vector3(0, 1f, 0);
        heldObject.transform.localScale = initialScale;
        heldObject.transform.localRotation = Quaternion.Euler(0, guide.transform.rotation.eulerAngles.y, 0);
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        heldObject.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;//((yeet) ? speed : speed * 0.1f);

        StartCoroutine(yeetEnableCollider(0.15f, heldObject.transform));
        checkOnDropped = heldObject;
        heldObject = null;

        if(PlankImg) PlankImg.active = false;
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

        if(PlankImg) PlankImg.active = false;
        checkOnDropped = null;
    }


    private IEnumerator yeetEnableCollider(float delay, Transform obj)
    {
        yield return new WaitForSeconds(delay);
        obj.gameObject.GetComponent<Collider>().enabled = true;
    }

    private IEnumerator disableGravAfterPlace(float delay, Transform obj)
    {
        yield return new WaitForSeconds(delay);
        obj.gameObject.GetComponent<Rigidbody>().useGravity = false;
    }


    private IEnumerator unparentAndReenable(float delay, Transform srslyunity)
    {
        yield return new WaitForSeconds(delay);
        heldObject.GetComponent<Collider>().enabled = true;
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        heldObject.tag = "Event";

        //heldObject.transform.SetParent(srslyunity);

        if (guide.GetChild(0) != null)
        {
            guide.GetChild(0).parent = null;
        }

        DontDestroyOnLoad(heldObject);
        StartCoroutine(disableGravAfterPlace(2f, heldObject.transform));
        heldObject = null;

        srslyunity.localPosition += new Vector3(0, -1f, 0);
        isPlacing = false;
    }
}