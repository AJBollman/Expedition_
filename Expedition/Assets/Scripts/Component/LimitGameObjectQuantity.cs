using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class LimitGameObjectQuantity : MonoBehaviour
{
    [SerializeField] private string identifier = "AnythingElseWithThisNameWillBeDestroyed";
    [SerializeField] private int limit = 1;
    [SerializeField] private bool dontDestroy;
    [SerializeField] private bool doLogWarning = true;

    private static Dictionary<string, int> list = new Dictionary<string, int>();
    private int hash;

   private void Awake()
    {
        hash = Random.Range(10000, 99999);
        if(!list.ContainsKey(identifier)) list[identifier] = hash;
        if (list[identifier] != hash)
        {
            Destroy(gameObject);
            if(doLogWarning) Debug.LogWarning("Removed one duplicate '"+identifier+"' ("+gameObject.name+") from the scene");
        }
        else if(dontDestroy) DontDestroyOnLoad(gameObject);
        /*foreach(var derp in list.Keys) {
            Debug.Log(derp + " " + list[derp]);
        }*/
    }
}
