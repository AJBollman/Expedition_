using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerDropThePlank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
		Debug.Log("jg");
        if(other.gameObject.tag == "Traveler")
        {
            var hold = other.gameObject.GetComponent<HoldItems>();
            if (hold == null) throw new System.Exception("Traveller entered the portal, but can't access his HoldItems component!");
            hold.Drop(false);
        }

    }

}
