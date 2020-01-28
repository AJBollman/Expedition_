using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireflies : MonoBehaviour
{
    public ParticleSystem Moths;
    public bool timerstart = false;
    public float lifeTime = 0f;
    public Collider player;

    private void Start()
    {
        Moths.enableEmission = false;
        Moths.Stop();
    }

    private void OnTriggerEnter(Collider player)
    {
        if(Moths.enableEmission == false)
        {
            Moths.enableEmission = true;
            Moths.Play();
        }
    }

    private void OnTriggerExit(Collider player)
    {
        if (Moths.enableEmission == true)
        {
            timerstart = true;
        }
    }

void Update()
    {
        if (timerstart == true)
        {
            lifeTime += Time.deltaTime;
        }

        if(lifeTime >= 10f)
        {
            timerstart = false;
            Moths.enableEmission = false;
            Moths.Stop();
            lifeTime = 0f;
        }
    }
}
