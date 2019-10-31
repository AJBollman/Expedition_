using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("World");
        Time.timeScale = 1f;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
