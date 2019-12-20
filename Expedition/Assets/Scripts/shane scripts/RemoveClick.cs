using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveClick : MonoBehaviour
{
    public GameObject destroyedVersion;
    public AudioSource sound1;
    public AudioSource sound2;
    public AudioSource lookingatSound;
    public bool stoplookingSound = false;
    public bool backonSound = false;
    public bool timerStart = false;
    public float lifeTime = 10f;

  

    void OnMouseDown()
    {
        timerStart = true;

        if (stoplookingSound == false)
        {
            lookingatSound.Play();
            stoplookingSound = true;
            backonSound = true;
        }
    }

    void OnMouseExit()
    {

        lookingatSound.Stop();
        stoplookingSound = false;
        backonSound = true;
    }

    void OnMouseOver()
    {
        if (timerStart == true)
        {
            
            lifeTime -= Time.deltaTime;
            
        }


        if (backonSound == true)
        {
            lookingatSound.Play();
            backonSound = false;
        }
        
        if (lifeTime <= 0)
        {
            lookingatSound.Stop();
            sound1.Play();
            sound2.Play();
            Instantiate(destroyedVersion, transform.position, transform.rotation);
            Destroy(gameObject);
            
        }

    }

}
