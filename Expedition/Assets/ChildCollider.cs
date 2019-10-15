using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    public bool isA;
    public bool isMid;
    void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<Transition>().OnDetect(isA);
    }

    private void OnCollisionExit(Collision collision)
    {
        transform.parent.GetComponent<Transition>().OnExit(isA);
    }
}