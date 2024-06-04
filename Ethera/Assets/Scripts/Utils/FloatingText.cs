using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float destroyTime = 0.4f;
    [SerializeField] [Tooltip("Range of random x position when FloatingText is created")] private float randomX = 0.5f;

    private void Start()
    {
        Destroy(gameObject, destroyTime);

        /// random position x only
        /// -x to x 
        transform.localPosition += new Vector3(Random.Range(-randomX, randomX), 0.0f, 0.0f);
    }
}