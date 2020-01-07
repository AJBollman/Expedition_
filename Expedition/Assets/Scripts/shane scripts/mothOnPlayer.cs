using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mothOnPlayer : MonoBehaviour
{
    public ParticleSystem Moths;
    
    void Start()
    {
        Moths.enableEmission = false;
        Moths.Stop();
    }
}
