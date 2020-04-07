using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicRock : MonoBehaviour
{
    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject item4;
    public GameObject item5;
    public int number = 0;
    public AudioSource treantSound;
    public ParticleSystem Poof;

    void OnMouseDown()
    {
        if (number == 2)
        {
            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = tempPosition;
            treantSound.Play();
            Poof.Play();
        }

        else if (number == 3)
        {
            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = item3.transform.position;
            item3.transform.position = tempPosition;
            treantSound.Play();
            Poof.Play();
        }

        else if (number == 4)
        {
            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = item3.transform.position;
            item3.transform.position = item4.transform.position;
            item4.transform.position = tempPosition;
            treantSound.Play();
            Poof.Play();
        }

        else if (number == 5)
        {
            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = item3.transform.position;
            item3.transform.position = item4.transform.position;
            item4.transform.position = item5.transform.position;
            item5.transform.position = tempPosition;
            treantSound.Play();
            Poof.Play();
        }

    }
}
