#if false
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEvent : MonoBehaviour
{
    public bool complete;
    public GameObject deleteOnComplete;

    void Start()
    {
        
    }

    private IEnumerator doDeleteOnComplete(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (deleteOnComplete != null) Destroy(deleteOnComplete);
    }

    public void completeEvent(GameObject obj)
    {
        GetComponent<MeshRenderer>().enabled = false;
        complete = true;
        StartCoroutine(doDeleteOnComplete(0.5f));
    }
}
#endif