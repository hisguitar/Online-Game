using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Player : NetworkBehaviour, ITakeDamage
{
    [Header("Settings")]
    [SerializeField] private NetworkVariable<float> maxHp = new();
    [SerializeField] private NetworkVariable<float> hp = new();

    [Header("Player Stats")]
    [SerializeField] private int playerStr = 10;
    //[SerializeField] private int playerDef = 10;
    //[SerializeField] private int playerAgi = 10;
    [SerializeField] private int playerVit = 10;
    //[SerializeField] private int playerInt = 10;

    [Header("Reference")]
    [SerializeField] private Image hpBar;

    private readonly int statsConvert = 10;
    private readonly float lerpSpeed = 3f;

    private void Start()
    {
        maxHp.Value = playerVit * statsConvert;
        hp.Value = maxHp.Value;
    }

    private void Update()
    {
        UIUpdate();
    }

    private void UIUpdate()
    {
        // Change color hp bar
        Color redFF5858 = new(1f, 0.345f, 0.345f);
        Color greenCFFF57 = new(0.78f, 1f, 0.341f);
        Color hpBarColor = Color.Lerp(redFF5858, greenCFFF57, hp.Value / maxHp.Value);
        hpBar.color = hpBarColor;

        // Fill hp bar
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp.Value / maxHp.Value, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);
    }
    
    // ITakeDamage Interface
    public void TakeDamage()
    {
        if (IsOwner)
        {
            TakeDamageServerRpc(playerStr);
        }
    }

    #region Networking
    // This client[Host] -> Server
    // ServerRpc can called by owner only.
    [ServerRpc]
    private void TakeDamageServerRpc(int amount)
    {
        // Call 'ClientRpc' to send data from Server -> all client[Client]
        TakeDamageClientRpc(amount);

        // The code below affects to owner(This game object) only.
        if (IsOwner)
        {
            hp.Value -= amount;
        }
    }

    // Server -> all client[Client]
    [ClientRpc]
    private void TakeDamageClientRpc(int amount)
    {
        // The code below affects all client[Client]
        // Except the owner[This game object]
        if (!IsOwner)
        {
            hp.Value -= amount;
        }
    }
    #endregion
}