using UnityEngine;

public class ChildCollider : MonoBehaviour
{
    public bool isA;
    public bool isMid;

    private void Awake() {
        if(gameObject.GetComponent<MeshRenderer>()) {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

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
            else
            {
                var a = other.gameObject.GetComponent<HoldItems>();
                if (a != null) a.Drop(false);
                transform.parent.GetComponent<Transition>().OnExit(isA);
            }
        }
    }
}