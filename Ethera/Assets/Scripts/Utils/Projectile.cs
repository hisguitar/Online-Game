using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private GameObject explosionPrefab;

    // Destroys itself when time runs out
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Do something when colliding with another GameObject
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.attachedRigidbody == null)
        {
            return;
        }
        else
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}