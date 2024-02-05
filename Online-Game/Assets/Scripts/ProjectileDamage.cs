using UnityEngine;

public class ProjectileDamage : MonoBehaviour
{
    // Do something when colliding with another GameObject
    private void OnTriggerEnter2D(Collider2D col)
    {
        switch (col.gameObject.tag)
        {
            case "Player":
                col.gameObject.GetComponent<ITakeDamage>().TakeDamage(25);
                Destroy(gameObject);
                break;
        }
    }
}