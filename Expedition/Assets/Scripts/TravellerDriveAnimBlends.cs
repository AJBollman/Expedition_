using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TravellerDriveAnimBlends : MonoBehaviour
{
    public string nameOfBlendParameter;
    private Animator a;
    private NavMeshAgent n;
    // Start is called before the first frame update
    void Start()
    {
        a = GetComponent<Animator>();
        n = transform.parent.gameObject.GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        var val = n.velocity.magnitude / n.speed;
        a.SetFloat(nameOfBlendParameter, val);
    }
}
