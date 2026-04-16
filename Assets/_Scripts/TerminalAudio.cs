using UnityEngine;

public class TerminalAudio : MonoBehaviour
{
    public AudioSource officeSpeaker;    // Para sa voices/SFX
    public AudioSource telephoneSource; // BAGONG SOURCE: I-set ito sa "Loop" sa Inspector

    public AudioClip goodCall, badCall, friendDay1, friendDay2, Day2EndingCall, IntrusionSFX, ringtone;

    public void PlayClip(AudioClip c)
    {
        if (officeSpeaker && c) { officeSpeaker.clip = c; officeSpeaker.Play(); }
    }

    // Function para i-control ang ringtone loop
    public void SetRingtone(bool state)
    {
        if (telephoneSource == null || ringtone == null) return;

        if (state)
        {
            telephoneSource.clip = ringtone;
            telephoneSource.loop = true; // Sinisiguradong mag-loop
            telephoneSource.Play();
        }
        else
        {
            telephoneSource.Stop();
        }
    }

    public float GetLength(AudioClip c) => c ? c.length : 2f;
}