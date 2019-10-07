using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private List<GameObject> regions = new List<GameObject>();
    private GameObject cross;
	private GameObject check;
	private Vector3 crossGoalPos;
	private Vector3 checkGoalPos;
	private Vector3 checkGoalScale;
	private bool complete;
    private bool discovered;
    private List<Region> ownedByRegions = new List<Region>();

    // Start is called before the first frame update
    void Start()
    {
        cross = transform.GetChild(0).gameObject;
		check = transform.GetChild(1).gameObject;
		crossGoalPos = new Vector3(0, -2f, 0);
		checkGoalPos = new Vector3(0, -2f, 0);
		checkGoalScale = new Vector3(1, 1, 1);
		cross.SetActive(false);
		check.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        cross.transform.localPosition = Vector3.Lerp(cross.transform.localPosition, crossGoalPos, Time.deltaTime);
        check.transform.localPosition = Vector3.Lerp(check.transform.localPosition, checkGoalPos, Time.deltaTime);
		check.transform.localScale = Vector3.Lerp(check.transform.localScale, checkGoalScale, Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        // Explorer enters.
        if(other.gameObject.name == "The Explorer")
        {
            StateController.activePortal = this;

            // Explorer sets the portal to discovered.
            discoverSequence();
        }
        // Traveller enters and this portal is not the active portal.
        else if(other.gameObject.tag == "Traveler" && StateController.activePortal != this)
		{
            discoverSequence();
			completeSequence();
		}
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "The Explorer")
        {
            StateController.activePortal = null;
        }
    }

    private void discoverSequence()
    {
        if (discovered) return;
        discovered = true;
        GetComponents<ParticleSystem>()[0].Play();
        crossGoalPos = Vector3.zero;
        cross.SetActive(true);
    }

    private void completeSequence()
	{
        if (complete) return;
        complete = true;
        check.SetActive(true);
		cross.SetActive(false);
		//GetComponents<ParticleSystem>()[1].Play();
		check.transform.localScale = new Vector3(1, 500, 1);
		checkGoalScale = new Vector3(1, 1, 1);
		checkGoalPos = Vector3.zero;
        foreach(Region x in ownedByRegions)
        {
            x.checkForCompletion();
        }
	}

    public void addOwnerRegion(Region region)
    {
        ownedByRegions.Add(region);
    }

    public bool isComplete()
    {
        return complete;
    }

}
