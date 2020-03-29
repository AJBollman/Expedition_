using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockBiomeCodex : MonoBehaviour
{
    public GameObject makeHidden;
    public AudioSource found;
    public Collider player;

    private void OnTriggerEnter(Collider player)
    {
        makeHidden.SetActive(false);
        found.Play();
    }
}
