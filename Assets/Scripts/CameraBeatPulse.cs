using UnityEngine;

/// <summary>
/// Camera zoom that continuously follows the volume of an isolated kick drum track.
/// 
/// Louder kick = more zoom in. Silence = no zoom.
/// No thresholds, no triggers — just smooth, direct audio-reactive zoom.
/// 
/// Setup:
///   - Add a second AudioSource with the isolated kick track.
///   - Set its volume to 0 (muted) — we read raw clip data, not output.
///   - Assign it to this script.
/// </summary>
public class CameraBeatPulse : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The main gameplay camera. If null, uses Camera.main.")]
    [SerializeField] private Camera targetCamera;

    [Tooltip("AudioSource playing the ISOLATED KICK track. " +
             "Volume can be 0 — detection reads raw clip data.")]
    [SerializeField] private AudioSource kickAudioSource;

    [Header("Audio Reading")]
    [Tooltip("Number of samples to read for RMS. 256 = ~5.8ms at 44100Hz.")]
    [SerializeField] private int sampleCount = 256;

    [Header("Zoom Effect")]
    [Tooltip("Base orthographic size. Leave at 0 to auto-detect from camera.")]
    [SerializeField] private float baseSize = 0f;

    [Tooltip("Maximum zoom amount at the loudest possible kick.")]
    [SerializeField] private float maxZoom = 0.15f;

    [Tooltip("Volume curve power. >1 = only strong kicks zoom noticeably. " +
             "<1 = even soft kicks cause zoom. 1 = linear.")]
    [SerializeField] [Range(0.5f, 4f)] private float curve = 2f;

    [Tooltip("How fast the zoom follows the volume UP (attack). Higher = snappier punch.")]
    [SerializeField] private float attackSpeed = 30f;

    [Tooltip("How fast the zoom returns to normal (release). Higher = quicker return.")]
    [SerializeField] private float releaseSpeed = 12f;

    // Internal
    private float[] sampleBuffer;
    private float currentZoom = 0f;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null && baseSize <= 0f)
            baseSize = targetCamera.orthographicSize;

        sampleBuffer = new float[sampleCount];
    }

    void Update()
    {
        if (targetCamera == null || kickAudioSource == null)
            return;

        float targetZoom = 0f;

        if (kickAudioSource.isPlaying)
        {
            // Read current volume of the kick track
            float rms = GetRMS();

            // Apply curve: pow > 1 makes quiet parts quieter, loud parts louder
            // This makes only strong kicks produce noticeable zoom
            float shaped = Mathf.Pow(rms, curve);

            // Map to zoom amount
            targetZoom = shaped * maxZoom;
        }

        // Smooth follow: fast attack (punch in), slower release (ease out)
        float speed = targetZoom > currentZoom ? attackSpeed : releaseSpeed;
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, speed * Time.unscaledDeltaTime);

        // Apply: smaller orthographicSize = zoom in
        targetCamera.orthographicSize = baseSize - currentZoom;
    }

    /// <summary>
    /// Reads RAW audio samples from the kick clip at the current playback position.
    /// Not affected by AudioSource.volume — works even when muted.
    /// Returns RMS (root mean square) = perceived loudness.
    /// </summary>
    private float GetRMS()
    {
        AudioClip clip = kickAudioSource.clip;
        if (clip == null) return 0f;

        int currentSample = kickAudioSource.timeSamples;
        int channels = clip.channels;
        int totalSamples = sampleCount * channels;

        if (currentSample + totalSamples > clip.samples * channels)
            return 0f;

        if (sampleBuffer.Length != totalSamples)
            sampleBuffer = new float[totalSamples];

        clip.GetData(sampleBuffer, currentSample);

        float sumSquares = 0f;
        for (int i = 0; i < sampleBuffer.Length; i++)
        {
            sumSquares += sampleBuffer[i] * sampleBuffer[i];
        }

        return Mathf.Sqrt(sumSquares / sampleBuffer.Length);
    }
}
