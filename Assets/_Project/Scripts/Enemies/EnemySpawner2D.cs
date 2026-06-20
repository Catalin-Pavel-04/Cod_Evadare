using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner2D : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int enemyCount = 3;

    public List<EnemyHealth> SpawnEnemies()
    {
        List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();

        if (enemyPrefab == null)
        {
            Debug.LogWarning($"EnemySpawner2D on '{name}' cannot spawn because enemyPrefab is missing.", this);
            return spawnedEnemies;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"EnemySpawner2D on '{name}' cannot spawn because no spawn points are assigned.", this);
            return spawnedEnemies;
        }

        int count = Mathf.Max(0, enemyCount);

        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            if (spawnPoint == null)
            {
                Debug.LogWarning($"EnemySpawner2D on '{name}' skipped a missing spawn point.", this);
                continue;
            }

            GameObject enemyObject = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation, transform.parent);
            EnemyHealth enemyHealth = enemyObject.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth = enemyObject.GetComponentInChildren<EnemyHealth>();
            }

            if (enemyHealth == null)
            {
                Debug.LogWarning($"Spawned enemy '{enemyObject.name}' is missing EnemyHealth.", enemyObject);
                continue;
            }

            spawnedEnemies.Add(enemyHealth);
        }

        Debug.Log($"EnemySpawner2D on '{name}' spawned {spawnedEnemies.Count} enemies.", this);
        return spawnedEnemies;
    }

    private void OnValidate()
    {
        enemyCount = Mathf.Max(0, enemyCount);
    }
}
