using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement using Unity's Input System.
/// Requires a Rigidbody2D component for physics-based movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f; // Movement speed multiplier

    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); } // Reference to Rigidbody2D
    private Vector2 moveDirection; // Current input direction

    // Applies velocity to the Rigidbody2D based on input direction and speed
    private void Update()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    /// <summary>
    /// Called by the Input System when movement input is received.
    /// Updates the move direction based on player input.
    /// </summary>
    /// <param name="context">The input context containing directional data.</param>
    public void Move(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
}