using UnityEngine;

namespace Vampire
{
    public class DecorationSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] decorationPrefabs;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float spawnRadius = 20f;
        [SerializeField] private float despawnRadius = 25f;
        [SerializeField] private int maxDecorations = 30;
        [SerializeField] private float spawnInterval = 1f;

        private float timeSinceLastSpawn;

        void Update()
        {
            timeSinceLastSpawn += Time.deltaTime;
            if (timeSinceLastSpawn >= spawnInterval && 
                GameObject.FindGameObjectsWithTag("Decoration").Length < maxDecorations)
            {
                SpawnDecoration();
                timeSinceLastSpawn = 0;
            }
            DespawnFarDecorations();
        }

        void SpawnDecoration()
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 spawnPos = playerTransform.position + 
                               new Vector3(randomDir.x, randomDir.y, 0) * spawnRadius;
            GameObject prefab = decorationPrefabs[Random.Range(0, decorationPrefabs.Length)];
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        void DespawnFarDecorations()
        {
            GameObject[] decorations = GameObject.FindGameObjectsWithTag("Decoration");
            foreach (GameObject deco in decorations)
            {
                if (Vector3.Distance(deco.transform.position, playerTransform.position) > despawnRadius)
                {
                    Destroy(deco);
                }
            }
        }
    }
}