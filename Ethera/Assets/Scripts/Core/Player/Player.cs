using UnityEngine;
using Unity.Netcode;
using Cinemachine;
using Unity.Collections;
using System;

public class Player : NetworkBehaviour
{
    [Header("Player settings")]
    // Character Creation
    public NetworkVariable<FixedString32Bytes> PlayerName = new();
    public NetworkVariable<int> PlayerColorIndex = new();

    [field: SerializeField] public PlayerHealth PlayerHealth { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private int ownerPriority = 15;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDespawned;

    // OnNetworkSpawn is used when an object begins network connection.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            /// If userData is null,
            /// it is possible that ApprovalCheck() in NetworkServer Not activated,
            /// the solution is to go to NetBootstrap scene
            /// and tick Connection Approval of NetworkManager to True.
            PlayerName.Value = userData.userName;
            PlayerColorIndex.Value = userData.userColorIndex;

            OnPlayerSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}