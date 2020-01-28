#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject plankIcon;
    private GameObject rockIcon;
    private bool iconShown;

    public void PlayGame()
    {
        //plankIcon = GameObject.Find("plankIcon");
        //rockIcon = GameObject.Find("rockIcon");
        //Expedition.StartGame();
        gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void showIcon(string item)
    {
        plankIcon.SetActive(false);
        rockIcon.SetActive(false);
        if (item == "Plank")
        {
            plankIcon.SetActive(true);
        }
        else if (item == "HoldableRock")
        {
            rockIcon.SetActive(true);
        }
    }
}
#endif
