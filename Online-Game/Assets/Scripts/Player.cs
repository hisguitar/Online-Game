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
        TakeDamageServerRpc(amount);
    }

    #region Networking
    [ServerRpc] // This client[Host] -> Server
    private void TakeDamageServerRpc(int amount)
    {
        // The code below effects all client[Client]
        // Call 'ClientRpc' to send data from Server -> all client[Client]
        TakeDamageClientRpc(amount);

        // The code below affects this client[Host] only.
        if (!IsOwner) return;
        hp.Value -= amount;
    }

    [ClientRpc] // Server -> all client[Client]
    private void TakeDamageClientRpc(int amount)
    {
        // The code below affects all client[Client] except this client[Host]
        if (IsOwner) return;
        hp.Value -= amount;
    }
    #endregion
}