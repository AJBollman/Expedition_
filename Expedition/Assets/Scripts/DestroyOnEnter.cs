using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEnter : MonoBehaviour
{
    public GameObject destroyedVersion;
    public Collider player;

    private void OnTriggerEnter(Collider player)
    {
        Destroy(destroyedVersion);
    }
}
