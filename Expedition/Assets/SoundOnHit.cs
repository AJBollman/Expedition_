using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnHit : MonoBehaviour
{
    public string hitSound;
    [Range( 0f, 2f)]
    public float maxVol;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.impulse.magnitude);
        GetComponent<SoundPlayer>().Play(hitSound, Mathf.Clamp(collision.impulse.magnitude, 0f, maxVol));
    }
}
