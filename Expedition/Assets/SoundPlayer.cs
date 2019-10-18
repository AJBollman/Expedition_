using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [System.Serializable]
    public struct AudThingy
    {
        public string name;
        public AudioClip[] soundsList;
        public float volume;
    }
    [SerializeField]
    public AudThingy[] sounds;



    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null) throw new System.Exception("SoundPlayer needs an audio source!");
    }



    public void Play(string name)
    {
        AudThingy sownd = new AudThingy();
        foreach (AudThingy t in sounds)
        {
            if (t.name == name)
            {
                sownd = t;
                break;
            }
        }
        var soundList = sownd.soundsList;
        int index = Random.Range(0, soundList.Length);
        audioSource.clip = soundList[index];
        audioSource.PlayOneShot(soundList[index], sownd.volume);
    }
    public void Play(string name, float vol)
    {
        AudThingy sownd = new AudThingy();
        foreach (AudThingy t in sounds)
        {
            if (t.name == name)
            {
                sownd = t;
                break;
            }
        }
        var soundList = sownd.soundsList;
        int index = Random.Range(0, soundList.Length);
        audioSource.clip = soundList[index];
        audioSource.PlayOneShot(soundList[index], vol);
    }
}
