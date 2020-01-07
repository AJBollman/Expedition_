using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingGround : MonoBehaviour
{

    public GameObject destroyedVersion;
    public Collider player;

    private void OnTriggerEnter(Collider player)
    {
        Instantiate(destroyedVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
