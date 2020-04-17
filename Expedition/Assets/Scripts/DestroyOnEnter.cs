using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEnter : MonoBehaviour
{
    public GameObject destroyedVersion;
    public GameObject QuestTracker;
    public GameObject thisBlock;
    public Collider player;

    private void OnTriggerEnter(Collider player)
    {
        Destroy(destroyedVersion);
        Destroy(QuestTracker);
        Destroy(thisBlock);
    }
}
