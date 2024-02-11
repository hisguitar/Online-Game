using UnityEngine;

public class Enemies : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolDistance = 1f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float chaseDistance = 5f;

    private GameObject player;
    private float distance;
    private Vector2 randomDirection;
    private Vector2 startPosition;

    private void Start()
    {
        startPosition = transform.position;
        randomDirection = GetRandomDirection();
    }

    private void Update()
    {
        EnemyLogic();
    }

    private void EnemyLogic()
    {
        // Find every GameObject with the tag "Player"
        GameObject[] playerGameObject = GameObject.FindGameObjectsWithTag("Player");

        // Select the closest Player.
        player = GetClosestPlayer(playerGameObject);
        if (player != null)
        {
            // Store origin and destination position in 'distance'
            distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < chaseDistance)
            {
                ChasePlayer(player);
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    // Patrol
    private void Patrol()
    {
        transform.position = Vector2.MoveTowards(transform.position, randomDirection, patrolSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, randomDirection) < 0.1f)
        {
            randomDirection = GetRandomDirection();
        }
    }

    // Chase player
    private void ChasePlayer(GameObject player)
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, player.transform.position) < 0.1f)
        {
            randomDirection = GetRandomDirection();
        }
    }

    // Get closest player position
    private GameObject GetClosestPlayer(GameObject[] players)
    {
        // Check players game object != null.
        GameObject closestPlayer = null;

        // Collects distance from enemy to the nearest player.
        float closestPlayerDistance = Mathf.Infinity;

        // loop through all GameObjects in the players array
        foreach (GameObject player in players)
        {
            // Calculates distance between the enemy's current position and the player's position.
            float distance = Vector2.Distance(transform.position, player.transform.position);
            // Check distance is less than the closest distance previously found. If yes, then continue.
            if (distance < closestPlayerDistance)
            {
                closestPlayerDistance = distance;
                closestPlayer = player;
            }
        }   
        return closestPlayer;
    }

    private Vector2 GetRandomDirection()
    {
        Vector2 direction = new Vector2(Random.Range(-patrolDistance, patrolDistance), Random.Range(-patrolDistance, patrolDistance)).normalized;
        return startPosition + direction * 3;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            // TakeDamage to player
        }
    }
}