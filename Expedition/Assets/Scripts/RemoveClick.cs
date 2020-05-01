using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveClick : MonoBehaviour
{
    public GameObject destroyedVersion;
    public GameObject destroyedVersionOther;
    public AudioSource sound1;
    public AudioSource sound2;
    public AudioSource lookingatSound;
    public bool stoplookingSound = false;
    public bool backonSound = false;
    public bool timerStart = false;
    public bool makeSound = false;
    public float lifeTime = 10f;

    public GameObject UnhideThis;
    public GameObject UnhideThis2;
    public GameObject UnhideThis3;

    public ParticleSystem dust;




    void OnMouseDown()
    {
        timerStart = true;

        if (stoplookingSound == false)
        {
            lookingatSound.Play();
            dust.Play();
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

    void OnMouseUp()
    {

        lookingatSound.Stop();
        stoplookingSound = false;
        backonSound = true;
        timerStart = false;
    }

    void OnMouseOver()
    {
        if (timerStart == true)
        {
            dust.Play();
            lifeTime -= Time.deltaTime;
        }


        if (backonSound == true)
        {
            //lookingatSound.Play();
            dust.Play();
            backonSound = false;
        }

      

        if (lifeTime <= 0)
        {
            lookingatSound.Stop();
            dust.Play();
            sound1.Play();
            sound2.Play();
          
            Instantiate(destroyedVersion, transform.position, transform.rotation);
            Destroy(gameObject);
            Destroy(destroyedVersionOther);
            UnhideThis.SetActive(true);
            UnhideThis2.SetActive(true);
            UnhideThis3.SetActive(true);
        }

    }

}
