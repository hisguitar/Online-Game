using System;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using Unity.Services.Authentication;

public class ClientGameManager : IDisposable
{
    private JoinAllocation allocation;
    private NetworkClient networkClient;
    public string JoinCode { get; private set; }

    private const string MenuSceneName = "Menu";
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthState authState = await AuthenticationWrapper.DoAuth();
        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            JoinCode = joinCode;
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            UserData userData = new()
            {
                userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
                userAuthId = AuthenticationService.Instance.PlayerId,
                userColorIndex = PlayerPrefs.GetInt(ColorSelector.PlayerColorKey, 0)
            };
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }
    }

    public void Disconnect()
    {
        networkClient.Disconnect();
    }

    public void Dispose()
    {
        networkClient?.Dispose();
    }
}