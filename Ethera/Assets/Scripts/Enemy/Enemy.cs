using TMPro;
using Unity.Collections;
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
    [Header("Reference")]
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject floatingTextPrefab;

    [Header("Enemy Stats")]
    private NetworkVariable<int> hp = new();
    private int EXPBounty;
    private int enemyStr;

    [Header("Enemy Patrol Stats")]
    private float patrolSpeed;
    private float patrolDistance;
    private float idleTime;

    [Header("Enemy Chase Stats")]
    private float chaseSpeed;
    private float chaseDistance;

    private ulong playerID;
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
        /// Get enemy stats from enemyData
        hp.Value = enemyData.hp;
        EXPBounty = enemyData.EXPBounty;
        enemyStr = enemyData.enemyStr;
        patrolSpeed = enemyData.patrolSpeed;
        patrolDistance = enemyData.patrolDistance;
        idleTime = enemyData.idleTime;
        chaseSpeed = enemyData.chaseSpeed;
        chaseDistance = enemyData.chaseDistance;
        ///

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
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null)
        {
            return;
        }

        // Deal damage to player
        if (col.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(enemyStr);
        }

        // Check owner of bullet when enemy TakeDamage()
        if (col.TryGetComponent(out DealDamageOnContact bullet))
        {
            playerID = bullet.OwnerClientId;
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
                if (NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject.TryGetComponent<PlayerHealth>(out PlayerHealth player))
                {
                    player.GainExp(EXPBounty);
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