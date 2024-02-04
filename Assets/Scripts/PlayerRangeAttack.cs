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

    private bool shouldFire;
    private float previousFireTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandlePrimaryFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    private void Update()
    {
        // Check is owner?
        if (!IsOwner) { return; } // If not owner, return.
        // Check is player shooting?
        if (!shouldFire) { return; } // If player is not shooting, return.
        // Check if fireRate is due?
        if (Time.time < previousFireTime + (1/fireRate)) { return; } // If not due, return.

        // If passed every condition
        // This client shooting!
        SpawnDummyProjectile(shootingPoint.position, shootingPoint.up);
        // Others client shooting!
        PrimaryFireServerRpc(shootingPoint.position, shootingPoint.up);

        // After shooting, set 'previousFireTime' to be 'shooting time'
        previousFireTime = Time.time;

    }

    // Dummy projectile shooting (Single player case)
    private void SpawnDummyProjectile(Vector3 shootingPos, Vector3 direction)
    {
        GameObject projectile = Instantiate(clientProjectilePrefab, shootingPos, Quaternion.identity);

        projectile.transform.up = direction;

        // Ignore player collision
        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());

        if(projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    // Server projectile shooting (Multiplayer case: This client -> Server)
    // 'This client' send data to 'Server' and 'Server' will send this data to 'Others client'
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 shootingPos, Vector3 direction)
    {
        GameObject projectile = Instantiate(serverProjectilePrefab, shootingPos, Quaternion.identity);

        projectile.transform.up = direction;

        // Ignore player collision
        Physics2D.IgnoreCollision(playerCollider, projectile.GetComponent<Collider2D>());

        if (projectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }

        // ServerRpc use ClientRpc method to get client data
        SpawnDummyProjectileClientRpc(shootingPos, direction);
    }

    // Client projectile shooting (Multiplayer case: Server -> Others client)
    // 'Server' send 'Others client' data to 'This client'
    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 shootingPos, Vector3 direction)
    {
        if (IsOwner) { return; }

        SpawnDummyProjectile(shootingPos, direction);
    }
}