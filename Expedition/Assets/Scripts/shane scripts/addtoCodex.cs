using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addtoCodex : MonoBehaviour
{
    public GameObject image;
    public GameObject NarrativeText;
    public AudioSource discover;
    public GameObject found;

    void OnMouseDown()
    {
        if(image.activeSelf == false)
        {
            image.SetActive(true);
            NarrativeText.SetActive(true);
            discover.Play();
            found.SetActive(false);
        }
      
    }
}
