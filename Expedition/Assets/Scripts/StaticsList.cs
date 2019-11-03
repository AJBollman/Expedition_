using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticsList : MonoBehaviour
{
    static List<GameObject> statics = new List<GameObject>();

    public static void destroyAll()
    {
        Transition.names = new List<string>();
        foreach (GameObject i in statics)
        {
            Destroy(i);
        }
        statics = new List<GameObject>();
        //SceneManager.LoadSceneAsync("World", LoadSceneMode.Single);
    }

    public static void add(GameObject g)
    {
        if (g == null) throw new System.Exception("Can't add to statics list, object doesn't exist!");
        statics.Add(g);
    }
}
