using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f; // Adjust the player's movement speed
    [SerializeField] private float smoothTime = 0.1f;  // Adjust the smooth time

    [Header("References")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InputReader inputReader;

    private Vector2 movementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; } // If it's not Owner, return;(go back and check again)
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; } // If it's not Owner, return;(go back and check again)
        inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 movementInput)
    {
        // this.movementInput refers to movementInput from this method, not from attributes.
        this.movementInput = movementInput;
    }

    private void Start()
    {
        // Auto reference to Rigidbody2D component.
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (IsOwner)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            Move();
        }
    }

    private void Move()
    {
        // Normalize the movement vector to ensure constant speed in all directions
        movementInput.Normalize();

        // Calculate the target velocity
        Vector2 targetVelocity = movementInput * moveSpeed;

        // Smoothly interpolate between the current velocity and the target velocity
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, smoothTime);
    }

    private void Flip()
    {
        // Walk to the right
        if (movementInput.x > 0)
        {
            playerSprite.flipX = false; // No flipping
        }
        // Walk to the left
        else if (movementInput.x < 0)
        {
            playerSprite.flipX = true; // Flip x-axis
        }
    }
}