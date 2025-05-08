using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private float smoothTime = 0.2f;  // Adjust the smooth time

    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Animator animator;

    private static readonly int OnMove = Animator.StringToHash("OnMove"); // Speed parameter in animator

    private Vector2 movementInput;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // If it's not Owner, return;(go back and check again)
        inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return; // If it's not Owner, return;(go back and check again)
        inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 movementInput)
    {
        // this.movementInput refers to movementInput from this method, not from attributes.
        this.movementInput = movementInput;
    }

    private void Update()
    {
        if (!IsOwner) return;
        Flip();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        Move();

        // Set the animator parameter based on whether the player is moving or not
        bool isMoving = IsMoving();
        animator.SetBool(OnMove, isMoving);
        UpdateAnimationServerRpc(isMoving);
    }

    #region Move
    private void Move()
    {
        // Normalize the movement vector to ensure constant speed in all directions
        movementInput.Normalize();

        // Calculate the target velocity
        Vector2 targetVelocity = movementInput * playerHealth.playerData.playerAgi;

        // Smoothly interpolate between the current velocity and the target velocity
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, smoothTime);
    }

    private bool IsMoving()
    {
        return movementInput != Vector2.zero;
    }

    [ServerRpc]
    private void UpdateAnimationServerRpc(bool isMoving)
    {
        UpdateAnimationClientRpc(isMoving);
    }

    [ClientRpc]
    private void UpdateAnimationClientRpc(bool isMoving)
    {
        animator.SetBool(OnMove, isMoving);
    }
    #endregion

    #region Flip
    private void Flip()
    {
        // Walk to the right
        if (movementInput.x > 0)
        {
            playerSprite.flipX = false; // No flipping
            FlipServerRpc(playerSprite.flipX);
        }
        // Walk to the left
        else if (movementInput.x < 0)
        {
            playerSprite.flipX = true; // Flip x-axis
            FlipServerRpc(playerSprite.flipX);
        }
    }

    [ServerRpc]
    private void FlipServerRpc(bool flipX)
    {
        FlipClientRpc(flipX);
    }

    [ClientRpc]
    private void FlipClientRpc(bool flipX)
    {
        playerSprite.flipX = flipX;
    }
    #endregion
}