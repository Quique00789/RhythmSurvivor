using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmNotes : MonoBehaviour
{
    public static RhythmNotes Instance;

    [Header("Referencias")]
    public RectTransform spawnLeft;
    public RectTransform spawnRight;
    public RectTransform hitLine;
    public RectTransform container;
    public AudioSource musicSource;

    [Header("Prefabs")]
    public GameObject bigNotePrefab;
    public GameObject smallNotePrefab;

    [Header("Ritmo")]
    public float bpm = 270f;

    [Header("Timing")]
    public float perfectRange = 10f;
    public float goodRange = 25f;

    float beatInterval;

    List<RectTransform> activeNotes = new List<RectTransform>();
    bool paused = false;

    public void SetPaused(bool value)
    {
        paused = value;
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        beatInterval = 60f / bpm;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Nota pequeña cada beat
            SpawnNotes(smallNotePrefab);

            // Nota grande cada 2 beats
            if (Mathf.FloorToInt(musicSource.time / beatInterval) % 2 == 0)
            {
                SpawnNotes(bigNotePrefab);
            }

            // Wait for the beat interval, but don't spawn while paused
            float waited = 0f;
            while (waited < beatInterval || paused)
            {
                yield return null;
                if (!paused)
                    waited += Time.unscaledDeltaTime;
            }
        }
    }

    void SpawnNotes(GameObject prefab)
    {
        CreateNote(prefab, spawnLeft.position);
        CreateNote(prefab, spawnRight.position);
    }

    void CreateNote(GameObject prefab, Vector3 pos)
    {
        GameObject note = Instantiate(prefab, container);
        RectTransform rect = note.GetComponent<RectTransform>();

        rect.position = pos;

        activeNotes.Add(rect);

        StartCoroutine(MoveNote(rect));
    }

    IEnumerator MoveNote(RectTransform note)
    {
        float duration = 0.5f;
        float t = 0;

        Vector3 start = note.position;
        Vector3 end = hitLine.position;

        while (t < 1)
        {
            // 🔥 FIX CRASH
            if (note == null)
                yield break;

            if (!paused)
            {
                t += Time.unscaledDeltaTime / duration;
                note.position = Vector3.Lerp(start, end, t);
            }
            yield return null;
        }

        if (note != null)
        {
            activeNotes.Remove(note);
            Destroy(note.gameObject);
        }
    }

    // =========================
    // 🎯 SISTEMA DE HIT
    // =========================

    public enum HitResult
    {
        Perfect,
        Good,
        Miss
    }

    public HitResult CheckHit(bool wantBigNote)
    {
        // 🔥 copia segura para evitar errores
        foreach (var note in new List<RectTransform>(activeNotes))
        {
            if (note == null) continue;

            var noteScript = note.GetComponent<RhythmNoteObject>();
            if (noteScript == null) continue;

            if (noteScript.alreadyHit) continue;

            if (wantBigNote && !noteScript.isBigNote) continue;

            float dist = Mathf.Abs(note.position.x - hitLine.position.x);

            if (dist < perfectRange)
            {
                noteScript.alreadyHit = true;
                activeNotes.Remove(note);
                Destroy(note.gameObject);
                return HitResult.Perfect;
            }
            else if (dist < goodRange)
            {
                noteScript.alreadyHit = true;
                activeNotes.Remove(note);
                Destroy(note.gameObject);
                return HitResult.Good;
            }
        }

        return HitResult.Miss;
    }
}