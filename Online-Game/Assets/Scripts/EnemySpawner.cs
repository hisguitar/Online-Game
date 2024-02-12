using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private void Update()
    {
        if (IsServer && Input.GetKeyDown(KeyCode.F))
        {
            SpawnEnemy(new Vector3(0, 0, 0));
        }
    }

    private void SpawnEnemy(Vector3 position)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();
    }
}