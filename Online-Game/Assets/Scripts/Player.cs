using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Player : NetworkBehaviour, ITakeDamage
{
    [Header("Settings")]
    [SerializeField] private NetworkVariable<float> maxHp = new(100f);
    [SerializeField] private NetworkVariable<float> hp = new(100f);

    [Header("Reference")]
    [SerializeField] private Image hpBar;

    private readonly float lerpSpeed = 3f;

    private void Update()
    {
        UIUpdate();
    }

    private void UIUpdate()
    {
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp.Value / maxHp.Value, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);
    }
    
    // ITakeDamage Interface
    public void TakeDamage(int amount)
    {
        if (IsOwner)
        {
            TakeDamageServerRpc(amount);
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