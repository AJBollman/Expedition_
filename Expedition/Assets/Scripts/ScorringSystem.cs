using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorringSystem : MonoBehaviour
{
    public GameObject scoreText;
    public static int theScore;
    

    void Update()
    {
        if(scoreText != null) scoreText.GetComponent<Text>().text = "Discoveries: " + theScore + " / 10"; 
    }
}
