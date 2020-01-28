using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class codexUI : MonoBehaviour
{
    public GameObject Panel;

    public GameObject Hide1;
    public GameObject Hide2;
    public GameObject Hide3;
    public GameObject Hide4;

    public void OpenPanel()
    {
        if(Panel != null)
        {
            Panel.SetActive(true);
            Hide1.SetActive(false);
            Hide2.SetActive(false);
            Hide3.SetActive(false);
            Hide4.SetActive(false);
        }
    }
}
