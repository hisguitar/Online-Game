using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.8f;

    public ulong OwnerClientId { get; private set; }
    private int PlayerStr;

    // Destroys itself when time runs out
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    /// To give Exp to the person who shoots last shot.
    /// Use SetOwner() to check owner of bullet.
    public void SetOwner(ulong ownerClientId, int playerPlayerStr)
    {
        this.OwnerClientId = ownerClientId;
        this.PlayerStr = playerPlayerStr;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null)
        {
            return;
        }

        #region TakeDamage to Enemy
        if (col.TryGetComponent(out Enemy enemy))
        {
            enemy.TakeDamage(PlayerStr);
            Destroy(gameObject);
        }
        #endregion

        #region TakeDamage to Player
        if (col.TryGetComponent(out NetworkObject netObj))
        {
            // If 'ClientId of bullet' = 'this player ClientId'
            if (OwnerClientId == netObj.OwnerClientId)
            {
                // then do nothing.
                return;
            }
        }

        if (col.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(PlayerStr);
            Destroy(gameObject);
        }
        #endregion
    }
}