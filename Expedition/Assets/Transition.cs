using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public Scene AScene;
    public Scene BScene;
    private bool isInside;
    private bool insideA;
    private bool insideB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnDetect(bool isA)
    {
        if (isA) insideA = true;
        else insideB = true;
        Debug.Log(isA);

        //if(insideA)
    }

    public void OnExit(bool isA)
    {
        if (isA) insideA = false;
        else insideB = false;
        Debug.Log(isA);
    }

}
