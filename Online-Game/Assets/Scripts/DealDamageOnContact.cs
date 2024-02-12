using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    private ulong ownerClientId;

    public void SetOwner(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        #region TakeDamage to Enemy
        if (col.attachedRigidbody.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.TakeDamage(10); // Need player.PlayerStr used instead of 10, But you also need to check who PlayerStr belongs to.
            Destroy(gameObject);
        }
        #endregion
        #region TakeDamage to Player
        if (col.attachedRigidbody == null) return;

        if (col.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject netObj))
        {
            if (ownerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        if (col.attachedRigidbody.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(player.PlayerStr);
            Destroy(gameObject);
        }
        #endregion
    }
}