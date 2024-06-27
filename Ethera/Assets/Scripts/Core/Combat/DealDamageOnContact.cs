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

    public void SetOwner(ulong ownerClientId, int playerPlayerStr)
    {
        this.OwnerClientId = ownerClientId;
        this.PlayerStr = playerPlayerStr; // Change this line
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
            if (OwnerClientId == netObj.OwnerClientId)
            {
                return;
            }
        }

        if (col.TryGetComponent(out Health player))
        {
            player.TakeDamage(PlayerStr);
            Destroy(gameObject);
        }
        #endregion
    }
}