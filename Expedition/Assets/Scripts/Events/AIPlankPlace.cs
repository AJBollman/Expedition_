using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlankPlace : MonoBehaviour
{
    public GameObject plank;
    public Transform guide;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Traveler")
        {
            plank.GetComponent<Rigidbody>().useGravity = false;
            plank.transform.position = guide.position;
            plank.transform.rotation = guide.rotation;
            plank.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            plank.transform.SetParent(guide);
            Destroy(this);
        }
    }
}
