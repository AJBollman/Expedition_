using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class geyserMovement3 : MonoBehaviour
{
    
    public bool timerstart = false;
    public bool begin = false;
    public float lifeTime = 0f;
    public AudioSource geyserSound;
    public ParticleSystem geyserfoam;

    void Update()
    {

        if (lifeTime >= 40f)
        {
            begin = true;
        }
        if (begin == true)
        {
            timerstart = true;
            lifeTime -= Time.deltaTime;
            geyserfoam.Stop();
            geyserfoam.enableEmission = false;
            // geyserSound.Stop();
        }
        if (begin == false)
        {
            timerstart = false;
            lifeTime += Time.deltaTime;
            geyserfoam.enableEmission = true;
            geyserfoam.Play();
            // geyserSound.Play();
        }

        if (lifeTime <= 0f)
        {
            begin = false;
        }


    }
}
