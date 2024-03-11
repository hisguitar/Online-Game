using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ConnectionsButtons : NetworkBehaviour
{
    // Players count variables
    [SerializeField] private TMP_Text playersCountText;
    private readonly NetworkVariable<int> playersNumber = new(0, NetworkVariableReadPermission.Everyone);

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost(); // Starts the NetworkManager as both a server and a client (that is, has local client)
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient(); // Starts the NetworkManager as just a client.
    }

    // Players count update
    private void Update()
    {
        playersCountText.text = "Players : " + playersNumber.Value.ToString();

        if (!IsServer) return;
        playersNumber.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }
}