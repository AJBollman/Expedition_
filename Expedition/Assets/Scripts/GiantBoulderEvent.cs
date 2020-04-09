using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantBoulderEvent : MonoBehaviour
{
    public GameObject TravellerUnlock;
    public GameObject PhoneyUnlock;
    public GameObject CrumbleUnlock;
    public GameObject FungalUnlock;
    public GameObject WispooUnlock;
    public GameObject GiantBoulder;

    void Update()
    {
        if (TravellerUnlock.activeSelf == true && PhoneyUnlock.activeSelf == true && CrumbleUnlock.activeSelf == true && FungalUnlock.activeSelf == true &&
            WispooUnlock.activeSelf == true)
        {
            GiantBoulder.SetActive(false);
        }
    }
}
