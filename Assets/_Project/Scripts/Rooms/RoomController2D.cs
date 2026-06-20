using System.Collections.Generic;
using UnityEngine;

public class RoomController2D : MonoBehaviour
{
    private enum RoomState
    {
        Inactive,
        Active,
        Cleared
    }

    [SerializeField] private DoorController2D[] doors;
    [SerializeField] private EnemySpawner2D enemySpawner;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private Transform rewardSpawnPoint;

    private readonly List<EnemyHealth> spawnedEnemies = new List<EnemyHealth>();
    private RoomState state = RoomState.Inactive;
    private bool rewardSpawned;

    public void ActivateRoom()
    {
        if (state != RoomState.Inactive)
        {
            Debug.Log($"Room '{name}' ignored activation because it is already {state}.", this);
            return;
        }

        state = RoomState.Active;
        Debug.Log($"Room '{name}' activated. Closing doors and spawning enemies.", this);

        CloseDoors();
        SpawnEnemies();
        CheckForRoomClear();
    }

    private void Update()
    {
        if (state != RoomState.Active)
        {
            return;
        }

        CheckForRoomClear();
    }

    private void CloseDoors()
    {
        if (doors == null)
        {
            return;
        }

        foreach (DoorController2D door in doors)
        {
            if (door != null)
            {
                door.CloseDoor();
            }
        }
    }

    private void OpenDoors()
    {
        if (doors == null)
        {
            return;
        }

        foreach (DoorController2D door in doors)
        {
            if (door != null)
            {
                door.OpenDoor();
            }
        }
    }

    private void SpawnEnemies()
    {
        spawnedEnemies.Clear();

        if (enemySpawner == null)
        {
            Debug.LogWarning($"Room '{name}' has no EnemySpawner2D assigned.", this);
            return;
        }

        List<EnemyHealth> enemies = enemySpawner.SpawnEnemies();

        if (enemies != null)
        {
            spawnedEnemies.AddRange(enemies);
        }

        Debug.Log($"Room '{name}' is tracking {spawnedEnemies.Count} spawned enemies.", this);
    }

    private void CheckForRoomClear()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);

        if (spawnedEnemies.Count > 0)
        {
            return;
        }

        ClearRoom();
    }

    private void ClearRoom()
    {
        if (state == RoomState.Cleared)
        {
            return;
        }

        state = RoomState.Cleared;
        Debug.Log($"Room '{name}' cleared. Opening doors.", this);

        OpenDoors();
        SpawnReward();
    }

    private void SpawnReward()
    {
        if (rewardSpawned)
        {
            return;
        }

        if (rewardPrefab == null)
        {
            Debug.Log($"Room '{name}' has no reward prefab assigned.", this);
            return;
        }

        if (rewardSpawnPoint == null)
        {
            Debug.LogWarning($"Room '{name}' has a reward prefab but no reward spawn point.", this);
            return;
        }

        rewardSpawned = true;
        Instantiate(rewardPrefab, rewardSpawnPoint.position, rewardSpawnPoint.rotation);
        Debug.Log($"Room '{name}' spawned reward '{rewardPrefab.name}'.", this);
    }
}
