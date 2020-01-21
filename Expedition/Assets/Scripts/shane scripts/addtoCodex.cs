using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class addtoCodex : MonoBehaviour
{
    public GameObject image;
    public GameObject NarrativeText;
    public AudioSource discover;

    void OnMouseDown()
    {
       image.SetActive(true);
       NarrativeText.SetActive(true);
       discover.Play();
    }
}
