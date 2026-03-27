using UnityEngine;

public class ChordClickManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip click11;
    public AudioClip click12;
    public AudioClip click13;
    public AudioClip click14;

    public AudioClip click21;
    public AudioClip click22;
    public AudioClip click23;
    public AudioClip click24;

    public void PlayClick()
    {
        float t = musicSource.time;

        int chordIndex = Mathf.FloorToInt(t / 2f) % 4;

        if (t < 24f)
        {
            switch (chordIndex)
            {
                case 0: sfxSource.PlayOneShot(click11); break;
                case 1: sfxSource.PlayOneShot(click12); break;
                case 2: sfxSource.PlayOneShot(click13); break;
                case 3: sfxSource.PlayOneShot(click14); break;
            }
        }
        else
        {
            switch (chordIndex)
            {
                case 0: sfxSource.PlayOneShot(click21); break;
                case 1: sfxSource.PlayOneShot(click22); break;
                case 2: sfxSource.PlayOneShot(click23); break;
                case 3: sfxSource.PlayOneShot(click24); break;
            }
        }
    }
}