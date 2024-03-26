using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    /// <summary>
    /// Old EnemySpawner
    /// Press 'F' to spawn enemy
    /// </summary>
    //[SerializeField] private GameObject enemyPrefab;

    //private void Update()
    //{
    //    if (IsServer && Input.GetKeyDown(KeyCode.F))
    //    {
    //        SpawnEnemy(new Vector3(0, 0, 0));
    //    }
    //}

    //private void SpawnEnemy(Vector3 position)
    //{
    //    GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
    //    enemy.GetComponent<NetworkObject>().Spawn();
    //}

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float spawnInterval = 5f;

    [SerializeField] private int maxEnemies = 5;
    private int currentEnemies = 0;
    private List<GameObject> spawnedEnemies = new();

    private void Start()
    {
        if (IsServer)
        {
            StartCoroutine(SpawnPeriodically());
        }
    }

    // Spawn enemies by counting time when 'currentEnemies' < 'maxEnemies'
    private IEnumerator SpawnPeriodically()
    {
        while (true)
        {
            if (currentEnemies < maxEnemies)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnEnemy(RandomPosition());
            }
            else
            {
                yield return null;
            }
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemyObject = Instantiate(enemyPrefab, position, Quaternion.identity);
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Initialize(this);
        }
        enemyObject.GetComponent<NetworkObject>().Spawn();

        currentEnemies++;
        spawnedEnemies.Add(enemyObject);
    }

    private Vector3 RandomPosition()
    {
        Vector3 centerPosition = transform.position;
        return centerPosition + new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius));
    }

    // Start spawn enemies again if enemy dies
    public void EnemyDestroyed(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            currentEnemies--;
            spawnedEnemies.Remove(enemy);
        }
    }
}