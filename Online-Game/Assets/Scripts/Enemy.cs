using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float speed;

    private GameObject player;
    private float distance;
    private Vector2 randomDirection;

    // Start is called before the first frame update
    void Start()
    {
        randomDirection = GetRandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        player = GetClosestPlayer(players);
        if (player != null)
        {
            distance = Vector2.Distance(transform.position, player.transform.position);
            Vector2 direction = player.transform.position - transform.position;

            if(distance < 5)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, player.transform.position) < 0.1f)
                {
                    randomDirection = GetRandomDirection();
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(this.transform.position, randomDirection, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, randomDirection) < 0.1f)
                {
                    randomDirection = GetRandomDirection();
                }
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(this.transform.position, randomDirection, speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, randomDirection) < 0.1f)
            {
                randomDirection = GetRandomDirection();
            }
        }
    }

    private GameObject GetClosestPlayer(GameObject[] players)
    {
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;
        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    private Vector2 GetRandomDirection()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
