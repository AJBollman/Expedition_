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
        if (isA)
        {

        }
    }

    public void OnExit(bool isA)
    {
        if (isA)
        {
            if(isInside) // entered from A
            {
                Debug.Log("entered from A");
            }
            else
            {
                Debug.Log("exited from A");
            }
        }
        else
        {
            if (isInside) // entered from B
            {
                Debug.Log("entered from B");
            }
            else
            {
                Debug.Log("exited from B");
            }
        }
    }

    public void OnMid(bool isIn)
    {
        isInside = isIn;
    }

}
