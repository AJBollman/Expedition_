using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{

    void Start()
    {
        Application.targetFrameRate = 60; 
    }

}
