using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverEvent : MonoBehaviour
{

    public GameObject plank;
    public Transform guide;
    public GameObject fakePlank;
    Vector3 startpos;
    Quaternion startrot;

    private void Awake()
    {
        guide = fakePlank.transform;
        startpos = plank.transform.position;
        startrot = plank.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (plank.transform.position.y < startpos.y - 100f)
        {
            plank.transform.position = startpos += new Vector3(0, 1f, 0);
            plank.transform.rotation = startrot;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, 3f))
            {
                if (hit.transform.gameObject.tag == "Event" && Vector3.Distance(plank.transform.position, fakePlank.transform.position) < 10f)
                {
                    Destroy(fakePlank);
                    place();
                }
            }
        }
    }
    void place()
    {
        //Set gravity to false while holding it
        plank.transform.position = new Vector3(guide.position.x, guide.position.y + 1f, guide.position.z);
        //plank.GetComponent<Rigidbody>().useGravity = false;
        plank.transform.rotation = guide.rotation;
        //plank.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        plank.transform.tag = "Untagged";
    }
}
