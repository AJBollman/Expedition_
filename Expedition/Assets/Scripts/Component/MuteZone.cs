using UnityEngine;

public class MuteZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Expedition.SetMusicMute(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Expedition.SetMusicMute(false);
        }
    }
}