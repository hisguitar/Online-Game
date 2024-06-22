using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;

    private void Start()
    {
        joinCodeText.text = HostSingleton.Instance.GameManager.JoinCode;
    }

    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.GameManager.Shutdown();
        }

        ClientSingleton.Instance.GameManager.Disconnect();
    }
}