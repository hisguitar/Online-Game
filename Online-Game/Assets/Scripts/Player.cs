using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private NetworkVariable<float> maxHp = new(100f);
    [SerializeField] private NetworkVariable<float> hp = new(100f);

    private readonly float lerpSpeed = 3f;

    private void Update()
    {
        if (IsOwner)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                TakeDamageServerRpc(25f);
            }
        }

        UIUpdate();
    }

    private void UIUpdate()
    {
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, hp.Value / maxHp.Value, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);
    }

    // This client[Host] -> Server
    [ServerRpc]
    private void TakeDamageServerRpc(float amount)
    {
        // Call 'ClientRpc' to send data from Server -> all client[Client]
        TakeDamageClientRpc(amount);

        // The code below affects Owner[Host] only.
        if (!IsOwner) return;
        hp.Value -= amount;
    }

    // Server -> all client[Client]
    [ClientRpc]
    private void TakeDamageClientRpc(float amount)
    {
        // The code below affects Client[Client] only.
        if (IsOwner) return;
        hp.Value -= amount;
    }
}