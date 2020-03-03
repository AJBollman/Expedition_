using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class glowWorm : MonoBehaviour
{
    public float min = 0f;
    public float max = 5f;

    public bool timerStart = false;
    public float lifeTime = 0f;

    public float lightstep = 10f;

    public Light glowLight;

    

    void OnMouseDown()
    {
        timerStart = true;
    }

    void OnMouseOver()
    {
        if (timerStart == true && glowLight.intensity < max)
        {
            lifeTime += Time.deltaTime;
            glowLight.intensity += lightstep * Time.deltaTime;
            
        }
    }

    void OnMouseUp()
    {
        timerStart = false;
    }

    void OnMouseExit()
    {
        timerStart = false;
    }

    void Update()
    {
        if (timerStart == false && glowLight.intensity > min)
        {
            lifeTime -= Time.deltaTime;
            glowLight.intensity -= lightstep/30 * Time.deltaTime;
        }
    }

}
