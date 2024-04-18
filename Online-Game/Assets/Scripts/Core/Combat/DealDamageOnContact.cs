using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    public ulong ownerClientId { get; private set; }
    private int PlayerStr;

    public void SetOwner(ulong ownerClientId, int playerPlayerStr)
    {
        this.ownerClientId = ownerClientId;
        this.PlayerStr = playerPlayerStr; // Change this line
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null)
        {
            return;
        }

        #region TakeDamage to Enemy
        if (col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(PlayerStr);
            Destroy(gameObject);
        }
        #endregion
        #region TakeDamage to Player

        if (col.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        if (col.TryGetComponent<Health>(out Health player))
        {
            player.TakeDamage(player.PlayerStr);
            Destroy(gameObject);
        }
        #endregion
    }
}