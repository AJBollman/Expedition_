using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideUnhideTime : MonoBehaviour
{

    public bool timerstart = false;
    public bool begin = false;
    public float lifeTime = 0f;
    public AudioSource geyserSound;
    public GameObject geyserCollider;
   


    void Update()
    {

        if (lifeTime >= Random.Range(22.0f, 64.0f))
        {
            begin = true;
        }
        if (begin == true)
        {
            timerstart = true;
            lifeTime -= Time.deltaTime;
            geyserCollider.SetActive(false);
          
            geyserSound.Stop();
        }
        if (begin == false)
        {
            timerstart = false;
            lifeTime += Time.deltaTime;
            geyserCollider.SetActive(true);
           
            geyserSound.Play();
        }

        if (lifeTime <= 0f)
        {
            begin = false;
        }


    }
}
