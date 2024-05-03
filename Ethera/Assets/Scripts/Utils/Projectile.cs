using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;

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
        {
            Destroy(gameObject);
        }
    }
}