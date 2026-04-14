using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    public static RhythmManager Instance;

    public AudioSource music;

    public float bpm = 270f;
    public float hitWindow = 0.07f;

    float beatInterval;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        beatInterval = 60f / bpm;
    }

    public bool IsOnBeat()
    {
        float songTime = music.time;

        float nearestBeat = Mathf.Round(songTime / beatInterval) * beatInterval;

        float difference = Mathf.Abs(songTime - nearestBeat);

        return difference < hitWindow;
    }
}