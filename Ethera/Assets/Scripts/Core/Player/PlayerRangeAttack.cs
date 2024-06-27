using Unity.Netcode;
using UnityEngine;

public class PlayerRangeAttack : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float fireRate = 4f;

    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Health player; // Add this line

    private bool shouldFire;
    private float previousFireTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void Update()
    {
        // Check is owner?, if not owner then do nothing.
        if (!IsOwner) return;
        // Check is player shooting?, if not then do nothing.
        if (!shouldFire) return;
        // Check if fireRate is due?, if not then do nothing.
        if (Time.time < previousFireTime + (1/fireRate)) return;

        // If passed every condition above
        // This client shooting! (other client can't see this bullet) but..
        SpawnDummyProjectile(shootingPoint.position, shootingPoint.up);
        // Others client see this bullet instead!
        PrimaryFireServerRpc(shootingPoint.position, shootingPoint.up);

        // After shooting, set 'previousFireTime' to be 'shooting time'
        previousFireTime = Time.time;
    }

    #region Spawn Projectile
    // Dummy projectile shooting (Single player case)
    private void SpawnDummyProjectile(Vector3 shootingPos, Vector3 direction)
    {
        GameObject projectile = Instantiate(clientProjectilePrefab, shootingPos, Quaternion.identity);
        projectile.transform.up = direction;

        // Ignore player(owner of bullet) collision
        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());
        if(projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    // This client[Host] -> Server
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 shootingPos, Vector3 direction)
    {
        GameObject projectile = Instantiate(serverProjectilePrefab, shootingPos, Quaternion.identity);
        projectile.transform.up = direction;

        // Ignore player collision
        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());
        if (projectile.TryGetComponent(out DealDamageOnContact dealDamage))
        {
            // Send 'ClientId of bullet' to 'DealDamageOnContact.cs'
            dealDamage.SetOwner(OwnerClientId, player.PlayerStr);
        }

        if (projectile.TryGetComponent(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        // Call 'ClientRpc' to send data from Server -> all client[Client]
        SpawnDummyProjectileClientRpc(shootingPos, direction);
    }

    // Server -> all client[Client]
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 shootingPos, Vector3 direction)
    {
        // The code below affects Client[Client] only.
        if (IsOwner) return;
        SpawnDummyProjectile(shootingPos, direction);
    }
    #endregion
}
