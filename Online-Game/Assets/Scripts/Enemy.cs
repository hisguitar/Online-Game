using Unity.Netcode;
using UnityEngine;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase
}

public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolDistance = 1f;
    [SerializeField] private float idleTime = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float chaseDistance = 5f;

    public int EnemyStr { get; private set; } = 10;

    private float countIdleTime = 0f;
    private float distance;
    private EnemyState state;
    private Vector2 randomDirection;
    private Vector2 startPosition;
    private GameObject player;

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
            // Play 'Idle' animation
            countIdleTime += Time.deltaTime;
        }
    }
    #endregion

    #region Patrol
    private void Patrol()
    {
        if (Vector2.Distance(transform.position, randomDirection) > 0.1f)
        {
            // Play 'Walk' animation
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
        if (player == null) return;

        // Play 'Walk' animation
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);

        // If target distance > 5f
        if (Vector2.Distance(transform.position, player.transform.position) > 5f)
        {
            state = EnemyState.Idle;
        }
    }
    #endregion

    #region Deal damage to player
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null) return;

        // Take Damage to player here!
        if (col.attachedRigidbody.TryGetComponent<Player>(out Player player))
        {
            player.TakeDamage(EnemyStr);
        }
    }
    #endregion
}