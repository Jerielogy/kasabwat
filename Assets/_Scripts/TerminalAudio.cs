using UnityEngine;

public class TerminalAudio : MonoBehaviour
{
    public AudioSource officeSpeaker;
    public AudioSource telephoneSource;

    [Header("New Audio Clips")]
    public AudioClip typingClip;
    public AudioClip interactClip;

    // Existing clips
    public AudioClip goodCall, badCall, friendDay1, friendDay2, Day2EndingCall, IntrusionSFX, ringtone;

    public void PlayClip(AudioClip c) { if (officeSpeaker && c) { officeSpeaker.clip = c; officeSpeaker.Play(); } }

    // FIXED: Para sa bawat letra sa terminal
    public void PlayTypingSound()
    {
        if (officeSpeaker && typingClip)
        {
            officeSpeaker.pitch = Random.Range(0.9f, 1.1f); // Kaunting variation
            officeSpeaker.PlayOneShot(typingClip);
        }
    }

    // FIXED: Para sa pag-click ng gamit
    public void PlayInteractSound()
    {
        if (officeSpeaker && interactClip)
        {
            officeSpeaker.pitch = 1.0f;
            officeSpeaker.PlayOneShot(interactClip);
        }
    }

    public void SetRingtone(bool state)
    {
        if (telephoneSource == null || ringtone == null) return;
        if (state) { telephoneSource.clip = ringtone; telephoneSource.loop = true; telephoneSource.Play(); }
        else { telephoneSource.Stop(); }
    }

    public float GetLength(AudioClip c) => c ? c.length : 2f;
}