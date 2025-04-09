using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement using Unity's Input System.
/// Requires a Rigidbody2D component for physics-based movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Normal movement speed multiplier
    public float dashSpeed = 10.0f; // Dash speed multiplier
    public float dashDuration = 0.2f; // Duration of dash
    public float dashCooldown = 1.0f; // Cooldown time between dashes

    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); } // Reference to Rigidbody2D
    private Vector2 moveDirection; // Current input direction
    private bool isDashing = false; // To track whether the player is currently dashing
    private float dashTime = 0f; // Timer for dash duration
    private float cooldownTime = 0f; // Timer for dash cooldown

    // Applies velocity to the Rigidbody2D based on input direction and speed
    private void Update()
    {
        // Apply regular movement speed if not dashing
        if (!isDashing)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
            // Handle cooldown timer
            if (cooldownTime > 0f)
            {
                cooldownTime -= Time.deltaTime;
            }
        }

        // Handle dash timer
        if (isDashing)
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f) // Dash has finished
            {
                isDashing = false;
            }
        }
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

    /// <summary>
    /// Called when the dash action is triggered by the input system.
    /// Starts the dash and applies dash speed temporarily.
    /// </summary>
    /// <param name="context">The input context for the dash action.</param>
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && cooldownTime <= 0f) // Start dash if the dash button is pressed and cooldown has passed
        {
            isDashing = true;
            dashTime = dashDuration; // Set the dash duration
            cooldownTime = dashCooldown; // Reset the cooldown
            rb.linearVelocity += moveDirection * dashSpeed; // Apply the dash speed in the move direction
        }
    }
}