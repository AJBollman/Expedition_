using UnityEngine;

public class MuteZone : MonoBehaviour
{
    [SerializeField] private bool isTransition;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Expedition.SetMusicMute(false, isTransition);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Expedition.SetMusicMute(false, isTransition);
        }
    }
}