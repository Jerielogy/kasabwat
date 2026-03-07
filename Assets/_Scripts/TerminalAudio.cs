using UnityEngine;

public class TerminalAudio : MonoBehaviour
{
    public AudioSource officeSpeaker;
    public AudioClip goodCall, badCall, friendDay1, friendDay2;

    public void PlayClip(AudioClip clip)
    {
        if (officeSpeaker != null && clip != null)
        {
            officeSpeaker.clip = clip;
            officeSpeaker.Play();
        }
    }

    public float GetLength(AudioClip clip) => clip != null ? clip.length : 2f;
}