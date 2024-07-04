using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private float keptExpPercentage;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        foreach (Player player in players)
        {
            HandlePlayerSpawned(player);
        }
        Player.OnPlayerSpawned += HandlePlayerSpawned;
        Player.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }
        Player.OnPlayerSpawned -= HandlePlayerSpawned;
        Player.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(Player player)
    {
        player.PlayerHealth.OnDie += (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDespawned(Player player)
    {
        player.PlayerHealth.OnDie -= (health) => HandlePlayerDie(player);
    }

    private void HandlePlayerDie(Player player)
    {
        // Keep player data, before die
        FixedString32Bytes playerName = player.PlayerName.Value;
        int playerColorIndex = player.PlayerColorIndex.Value;
        int keptExp = (int)(player.PlayerHealth.Exp.Value * (keptExpPercentage / 100));        

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, playerName, playerColorIndex, keptExp));
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, FixedString32Bytes playerName, int playerColorIndex, int keptExp)
    {
        yield return null;

        // Spawn Player
        Player playerInstance = Instantiate(
            playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);

        // Modify userData(from data before die) PlayerName, PlayerColorIndex, Exp.Value
        playerInstance.PlayerName.Value = playerName;
        playerInstance.PlayerColorIndex.Value = playerColorIndex;
        playerInstance.PlayerHealth.Exp.Value += keptExp;
    }
}