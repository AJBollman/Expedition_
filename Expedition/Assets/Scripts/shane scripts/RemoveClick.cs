using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveClick : MonoBehaviour
{
    public GameObject destroyedVersion;
    public AudioSource clipsound;
    public bool timerStart = false;
    public float lifeTime = 10f;

  

    void OnMouseDown()
    {
        timerStart = true;
    }



    void OnMouseOver()
    {
        if (timerStart == true) { lifeTime -= Time.deltaTime; }

        if (lifeTime <= 0)
        {
            clipsound.Play();
            Instantiate(destroyedVersion, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }

}
