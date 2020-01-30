/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPortal : MonoBehaviour
{
    private bool discovered;

    // Start is called before the first frame update
    void Start()
    {
        setColor(color);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "The Explorer")
        {
            discoverSequence();
        }
    }

    void setColor(Color color)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetColor("_BaseColor", color);
        GetComponent<Renderer>().SetPropertyBlock(block);
        gameObject.GetComponent<Renderer>().SetPropertyBlock(block);
    }



}*/
