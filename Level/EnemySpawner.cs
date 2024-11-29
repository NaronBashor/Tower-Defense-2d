using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Array to hold the 9 prefabs
    public Transform spawnPoint; // The point where the prefab will be spawned
    public float timer;

    private void Start()
    {
        if (prefabs.Length == 0) {
            Debug.LogError("No prefabs assigned to the spawner.");
            return;
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            SpawnRandomPrefab();
            timer = 2;
        }
    }

    public void SpawnRandomPrefab()
    {
        if (prefabs.Length == 0) {
            Debug.LogWarning("No prefabs to spawn.");
            return;
        }

        // Choose a random prefab index
        int randomIndex = Random.Range(0, prefabs.Length);

        // Instantiate the chosen prefab at the spawn point's position and rotation
        Instantiate(prefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);
    }
}
