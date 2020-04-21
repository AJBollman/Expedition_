using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeInteractible : MonoBehaviour
{

    public GameObject item1;
    public GameObject item2;
    public GameObject item3;
    public GameObject item4;
    public GameObject item5;
    public int number = 0;
    public AudioSource treantSound;

    public ParticleSystem part1;
    public ParticleSystem part2;
    public ParticleSystem part3;
    public ParticleSystem part4;

    public ParticleSystem puff1;
    public ParticleSystem puff2;
    public ParticleSystem puff3;
    public ParticleSystem puff4;

    private Vector3 scaleChange;

    private void Awake()
    {
        scaleChange = new Vector3(-.06f, -.06f, -.06f);
    }

    void OnMouseDown()
    {
        if (number == 2)
        {
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();

            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = tempPosition;

           // item1.transform.localScale += scaleChange;
            //item2.transform.localScale += scaleChange;
            
           // if (item1.transform.localScale.y < 0.1f || item1.transform.localScale.y > 1.0f)
           // {
                
           //     scaleChange = -scaleChange;
           // }
            treantSound.Play();
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();
        }

        else if (number == 3)
        {
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();

            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = item3.transform.position;
            item3.transform.position = tempPosition;
           // item1.transform.localScale += scaleChange;
           // item2.transform.localScale += scaleChange;
           // item3.transform.localScale += scaleChange;
           
           // if (item1.transform.localScale.y < 0.1f || item1.transform.localScale.y > 1.0f)
           // {
                
          //      scaleChange = -scaleChange;
          //  }
            treantSound.Play();
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();
        }

        else if (number == 4)
        {
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();
            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = item3.transform.position;
            item3.transform.position = item4.transform.position;
            item4.transform.position = tempPosition;

           // item1.transform.localScale += scaleChange;
           // item2.transform.localScale += scaleChange;
           // item3.transform.localScale += scaleChange;
            //item4.transform.localScale += scaleChange;
            
           // if (item1.transform.localScale.y < 0.1f || item1.transform.localScale.y > 1.0f)
           // {
                
           //     scaleChange = -scaleChange;
           // }
            treantSound.Play();
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();
        }

        else if (number == 5)
        {
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();
            Vector3 tempPosition = item1.transform.position;
            item1.transform.position = item2.transform.position;
            item2.transform.position = item3.transform.position;
            item3.transform.position = item4.transform.position;
            item4.transform.position = item5.transform.position;
            item5.transform.position = tempPosition;

           // item1.transform.localScale += scaleChange;
            //item2.transform.localScale += scaleChange;
           // item3.transform.localScale += scaleChange;
           // item4.transform.localScale += scaleChange;
           // item5.transform.localScale += scaleChange;
           // if (item1.transform.localScale.y < 0.1f || item1.transform.localScale.y > 1.0f)
           // {
                
            //    scaleChange = -scaleChange;
            //}
            treantSound.Play();
            part1.Play();
            part2.Play();
            part3.Play();
            part4.Play();

            puff1.Play();
            puff2.Play();
            puff3.Play();
            puff4.Play();
        }

    }
}
