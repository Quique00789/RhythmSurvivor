using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RhythmNotes : MonoBehaviour
{
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

    float beatInterval;
    float timer;

    void Start()
    {
        beatInterval = 60f / bpm;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Beat normal (cada beat)
            SpawnNotes(smallNotePrefab);

            // Cada 2 beats → nota grande
            if (Mathf.FloorToInt(musicSource.time / beatInterval) % 2 == 0)
            {
                SpawnNotes(bigNotePrefab);
            }

            yield return new WaitForSeconds(beatInterval);
        }
    }

    void SpawnNotes(GameObject prefab)
    {
        CreateNote(prefab, spawnLeft.position, 1);
        CreateNote(prefab, spawnRight.position, -1);
    }

    void CreateNote(GameObject prefab, Vector3 pos, int dir)
    {
        GameObject note = Instantiate(prefab, container);
        note.GetComponent<RectTransform>().position = pos;

        StartCoroutine(MoveNote(note.GetComponent<RectTransform>(), dir));
    }

    IEnumerator MoveNote(RectTransform note, int dir)
    {
        float duration = 0.5f; // tiempo para llegar al centro
        float t = 0;

        Vector3 start = note.position;
        Vector3 end = hitLine.position;

        while (t < 1)
        {
            t += Time.deltaTime / duration;
            note.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        Destroy(note.gameObject);
    }
}