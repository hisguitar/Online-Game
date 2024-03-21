using TMPro;
using Unity.Netcode;
using UnityEngine;

public enum EnemyBatState
{
    Patrol,
    Chase,
    Attack
}

public class EnemyBat2 : NetworkBehaviour
{
    public int EnemyStr { get; private set; } = 5;
    [SerializeField] private NetworkVariable<int> hp = new();

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolDistance = 1f;
    [SerializeField] private float idleTime = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float attackDistance = 2f;

    [Header("Reference")]
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject floatingTextPrefab;

    private float countIdleTime = 0f;
    private float distance;
    private EnemyBatState state;
    private Vector2 randomDirection;
    private Vector2 startPosition;
    private GameObject player;

    private static readonly int OnMove = Animator.StringToHash("OnMove"); // Speed parameter in animator

    private void Start()
    {
        startPosition = transform.position;
        //randomDirection = GetRandomDirection();
        state = EnemyBatState.Patrol;
    }

    private void Update()
    {
        EnemyLogic();
    }

    private void EnemyLogic()
    {
        switch (state)
        {
            case EnemyBatState.Patrol:
                Patrol();
                break;
            case EnemyBatState.Chase:
                ChasePlayer(player);
                break;
            default:
                break;
        }

        // Search for targets during Idle / Patrol
        if (state == EnemyBatState.Patrol)
        {
            FindTarget();
        }

        if (state == EnemyBatState.Patrol || state == EnemyBatState.Chase)
        {
            animator.SetBool(OnMove, true);
        }
    }
    #region Attack
    private void Attack()
    {        
        distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < attackDistance)
        {
            state = EnemyBatState.Attack;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, 3 * Time.deltaTime);
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
            state = EnemyBatState.Chase;
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
            #region Flip
            Vector2 direction = player.transform.position - transform.position;

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
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);

            // If target distance > 5f
            /*if (Vector2.Distance(transform.position, player.transform.position) > 5f)
            {
                state = EnemyBatState.Idle;
            }*/
            #endregion
        }
        /*else
        {
            state = EnemyBatState.Idle;
        }*/
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