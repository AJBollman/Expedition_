using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    public bool isA;
    public bool isMid;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isMid) transform.parent.GetComponent<Transition>().OnMid(true);
            else transform.parent.GetComponent<Transition>().OnDetect(isA);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (isMid) transform.parent.GetComponent<Transition>().OnMid(false);
            else transform.parent.GetComponent<Transition>().OnExit(isA);
        }
    }
}