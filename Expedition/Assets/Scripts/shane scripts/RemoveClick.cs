using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveClick : MonoBehaviour
{
    public GameObject destroyedVersion;
    public AudioSource clipsound;



    void OnMouseDown()
    {
        clipsound.Play();
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    
    }
}
