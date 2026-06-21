using UnityEngine;

public class RoomLootSpawner2D : MonoBehaviour
{
    [SerializeField] private GameObject[] lootPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private bool spawnAll = true;
    [SerializeField] private int randomSpawnCount = 3;

    private bool hasSpawned;

    public void SpawnLoot()
    {
        if (hasSpawned)
        {
            return;
        }

        if (lootPrefabs == null || lootPrefabs.Length == 0)
        {
            Debug.LogWarning($"{name} cannot spawn loot because no loot prefabs are assigned.", this);
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"{name} cannot spawn loot because no spawn points are assigned.", this);
            return;
        }

        hasSpawned = true;

        if (spawnAll)
        {
            SpawnAllLoot();
        }
        else
        {
            SpawnRandomLoot();
        }
    }

    private void SpawnAllLoot()
    {
        for (int i = 0; i < lootPrefabs.Length; i++)
        {
            SpawnPrefabAtIndex(lootPrefabs[i], i);
        }
    }

    private void SpawnRandomLoot()
    {
        int count = Mathf.Max(0, randomSpawnCount);

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
            SpawnPrefabAtIndex(prefab, i);
        }
    }

    private void SpawnPrefabAtIndex(GameObject prefab, int index)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{name} skipped a missing loot prefab.", this);
            return;
        }

        Transform spawnPoint = spawnPoints[index % spawnPoints.Length];

        if (spawnPoint == null)
        {
            Debug.LogWarning($"{name} skipped a missing loot spawn point.", this);
            return;
        }

        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log($"{name} spawned loot '{prefab.name}'.", this);
    }

    private void OnValidate()
    {
        randomSpawnCount = Mathf.Max(0, randomSpawnCount);
    }
}
