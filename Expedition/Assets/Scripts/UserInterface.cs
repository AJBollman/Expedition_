using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum crosshairTypes { draw, yeet, drop, grab, place, nope, none };
public class UserInterface : MonoBehaviour
{
    private static GameObject crosshair;
    private static crosshairTypes currentCrosshair;
    private void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            StaticsList.add(gameObject);
        }

        crosshair = GameObject.Find("Crosshair");
    }

    public static void SetCursor(crosshairTypes type)
    {
        //if ((int)type != (int)currentCrosshair) return;
        //Debug.Log("called");

        switch (type)
        {
            case crosshairTypes.draw:
                {
                    crosshair.GetComponent<Text>().text = "O";
                    break;
                }
            case crosshairTypes.yeet:
                {
                    crosshair.GetComponent<Text>().text = "^";
                    break;
                }
            case crosshairTypes.drop:
                {
                    crosshair.GetComponent<Text>().text = "v";
                    break;
                }
            case crosshairTypes.grab:
                {
                    crosshair.GetComponent<Text>().text = "> <";
                    break;
                }
            case crosshairTypes.place:
                {
                    crosshair.GetComponent<Text>().text = "v";
                    break;
                }
            case crosshairTypes.nope:
                {
                    crosshair.GetComponent<Text>().text = "x";
                    break;
                }
            case crosshairTypes.none:
                {
                    crosshair.GetComponent<Text>().text = "";
                    break;
                }
            default:
            {
                break;
            }
        }
        currentCrosshair = type;
    }
}
