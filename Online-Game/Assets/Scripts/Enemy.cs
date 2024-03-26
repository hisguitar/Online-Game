using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase
}

public class Enemy : NetworkBehaviour
{
    public int EnemyStr { get; private set; } = 5;
    [SerializeField] private NetworkVariable<int> hp = new();
    [SerializeField] private int exp = 20;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolDistance = 1f;
    [SerializeField] private float idleTime = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float chaseDistance = 3f;

    [Header("Reference")]
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject floatingTextPrefab;

    private ulong playerId;
    private float countIdleTime = 0f;
    private float distance;
    private EnemyState state;
    private Vector2 randomDirection;
    private Vector2 startPosition;
    private GameObject player;
    private EnemySpawner enemySpawner;

    private static readonly int OnMove = Animator.StringToHash("OnMove"); // Speed parameter in animator

    private void Start()
    {
        startPosition = transform.position;
        randomDirection = GetRandomDirection();
        state = EnemyState.Idle;
    }

    private void Update()
    {
        EnemyLogic();
    }

    public void Initialize(EnemySpawner spawner)
    {
        enemySpawner = spawner;
    }

    private void EnemyLogic()
    {
        switch (state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                ChasePlayer(player);
                break;
            default:
                break;
        }

        // Search for targets during Idle / Patrol
        if (state == EnemyState.Idle || state == EnemyState.Patrol)
        {
            FindTarget();
        }

        if (state == EnemyState.Patrol || state == EnemyState.Chase)
        {
            // Set the animator parameter based on whether the enemy is moving or not
            animator.SetBool(OnMove, true);
        }
        else if (state == EnemyState.Idle)
        {
            // Set the animator parameter based on whether the enemy is moving or not
            animator.SetBool(OnMove, false);
        }
    }

    #region Idle
    private void Idle()
    {
        if (countIdleTime >= idleTime)
        {
            randomDirection = GetRandomDirection();
            state = EnemyState.Patrol;
            countIdleTime = 0;
        }
        else
        {
            countIdleTime += Time.deltaTime;
        }
    }
    #endregion

    #region Patrol
    private void Patrol()
    {
        if (Vector2.Distance(transform.position, randomDirection) > 0.1f)
        {
            #region Flip
            Vector2 direction = randomDirection - (Vector2)transform.position;

            // Walk to the left
            if (direction.x < 0)
            {
                Flip(true); // Flip x-axis
            }
            // Walk to the right
            else if (direction.x > 0)
            {
                Flip(false); // No flipping
            }
            #endregion

            transform.position = Vector2.MoveTowards(transform.position, randomDirection, patrolSpeed * Time.deltaTime);
        }
        else
        {
            state = EnemyState.Idle;
        }
    }

    private Vector2 GetRandomDirection()
    {
        Vector2 direction = new Vector2(Random.Range(-patrolDistance, patrolDistance), Random.Range(-patrolDistance, patrolDistance)).normalized;
        return startPosition + direction * 3;
    }
    #endregion

    #region Find Target
    private void FindTarget()
    {
        // Find every GameObject with the tag "Player"
        GameObject[] playerGameObject = GameObject.FindGameObjectsWithTag("Player");

        if (playerGameObject == null) return;

        // Select the closest Player
        player = GetClosestPlayer(playerGameObject);

        if (player == null ) return;

        // Calculate distance
        distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < chaseDistance)
        {
            state = EnemyState.Chase;
        }
        else
        {
            return;
        }
    }

    private GameObject GetClosestPlayer(GameObject[] players)
    {
        GameObject closestPlayer = null;

        float closestPlayerDistance = Mathf.Infinity;
        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestPlayerDistance)
            {
                closestPlayerDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }
    #endregion

    #region Chase
    // If found target
    private void ChasePlayer(GameObject player)
    {
        if (player != null)
        {
            Vector3 playerPosition = player.transform.position;
            playerPosition.y -= 0.2f; // -0.2 offset in y-axis

            #region Flip
            Vector2 direction = playerPosition - transform.position;

            // Walk to the left
            if (direction.x < 0)
            {
                Flip(true); // Flip x-axis
            }
            // Walk to the right
            else if (direction.x > 0)
            {
                Flip(false); // No flipping
            }
            #endregion
            #region Chase
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, chaseSpeed * Time.deltaTime);

            // If target distance > 5f

            if (Vector2.Distance(transform.position, player.transform.position) > 5f)
            {
                state = EnemyState.Idle;
            }
            #endregion
        }
        else
        {
            state = EnemyState.Idle;
        }
    }
    #endregion

    #region Flip
    private void Flip(bool flip)
    {
        // Walk to the right
        if (!flip)
        {
            enemySprite.flipX = false; // No flipping
        }
        // Walk to the left
        else if (flip)
        {
            enemySprite.flipX = true; // Flip x-axis
        }
    }
    #endregion

    #region Deal damage to player & Check owner of bullet
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D otherRigidbody = collision.attachedRigidbody;
        if (otherRigidbody == null) return;

        // Take Damage to player here!
        if (otherRigidbody.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(EnemyStr);
        }

        if (otherRigidbody.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact bullet))
        {
            playerId = bullet.ownerClientId;
        }
    }

    #endregion

    #region Take damage
    public void TakeDamage(int amount)
    {
        hp.Value -= amount;
        if (floatingTextPrefab != null)
        {
            ShowFloatingTextClientRpc($"-{amount}");
        }

        if (hp.Value <= 0)
        {
            if (IsServer)
            {
                Player player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();
                if (player != null)
                {
                    player.GainExp(exp);
                }
                enemySpawner.EnemyDestroyed(gameObject);
                NetworkObject.Despawn();
            }
        }
    }

    [ClientRpc]
    private void ShowFloatingTextClientRpc(string text)
    {
        ShowFloatingText(text);
    }
    #endregion

    #region Show Floating Text
    private void ShowFloatingText(string text)
    {
        GameObject go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.GetComponent<TMP_Text>().text = text;
    }
    #endregion
}