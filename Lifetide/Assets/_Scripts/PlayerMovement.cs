using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UiInfoStore;

/// <summary>
/// Handles player movement using Unity's Input System.
/// Requires a Rigidbody2D component for physics-based movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour, IUiReadable
{
    public float moveSpeed; // Normal movement speed multiplier
    public float shieldedMoveSpeed; // Normal movement speed multiplier
    public float dashSpeed; // Dash speed multiplier
    public float weakDashNerf; // Weak dash speed multiplier per extra dash
    public float dashDuration; // Duration of dash
    public float dashCooldown; // Cooldown time between dashes
    public float maxDashAmount; // Amount of dashes within 1 cooldown allowed
    public float maxWeakDashAmount; // Amount of dashes within 1 cooldown allowed

    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); } // Reference to Rigidbody2D
    private Vector2 moveDirection; // Current input direction
    private Vector2 dashDirection; // Remembered input direction
    private bool isDashing = false; // To track whether the player is currently dashing
    private float dashTime = 0f; // Timer for dash duration
    private float cooldownTime = 0f; // Timer for dash cooldown
    private float dashAmount = 1f; // Amount of dashes left

    private IWeaponReadable currrentWeapon;
    private IDamageable thisHealth;
    private IWeaponStatusable thisWeapon;

    private void Start()
    {
        dashAmount = maxDashAmount;

        thisHealth = GetComponent<IDamageable>();

        currrentWeapon = GetComponent<IWeaponReadable>();
        if (currrentWeapon != null) thisWeapon = currrentWeapon.weapon.GetComponent<IWeaponStatusable>();

        SetMovementStats();
    }

    private void Update()
    {
        ApplyMovement();

        // Handle dash timer
        if (isDashing)
        {
            if (dashAmount > 0)
            {
                rb.linearVelocity += dashDirection * dashSpeed; // Apply the dash speed in the move direction
            }
            else
            {
                rb.linearVelocity += dashDirection * (dashSpeed - dashSpeed * weakDashNerf * Mathf.Abs(dashAmount)); // Apply the weak dash speed in the move direction
            }

            dashTime -= Time.deltaTime;
            if (dashTime <= 0f) // Dash has finished
            {
                isDashing = false;
            }
        }
        else
        {
            // Handle cooldown timer
            if (cooldownTime > 0f)
            {
                cooldownTime -= Time.deltaTime;
            }
            else
            {
                dashAmount = maxDashAmount;
            }
        }
    }

    private void ApplyMovement()
    {
        if (currrentWeapon?.weaponStatus?.IsBlocking == true)
        {
            rb.linearVelocity = moveDirection * shieldedMoveSpeed;
        }
        else
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void SetMovementStats()
    {
        // If no weapon then use defaults
        if (thisWeapon == null || currrentWeapon == null)
        {
            Debug.LogWarning("Using default movement stats");
            moveSpeed = 15f;
            shieldedMoveSpeed = 7f;
            dashSpeed = 40f;
            weakDashNerf = 0.3f;
            dashDuration = 0.1f;
            dashCooldown = 2.5f;
            maxDashAmount = 3f;
            maxWeakDashAmount = 4f;
            return;
        }

        moveSpeed = thisWeapon.GetWeaponStats().moveSpeed; 
        shieldedMoveSpeed = thisWeapon.GetWeaponStats().shieldedMovement; 
        dashSpeed = thisWeapon.GetWeaponStats().dashSpeed; 
        weakDashNerf = thisWeapon.GetWeaponStats().weakDashNerf; 
        dashDuration = thisWeapon.GetWeaponStats().dashDuration; 
        dashCooldown = thisWeapon.GetWeaponStats().dashCooldown; 
        maxDashAmount = thisWeapon.GetWeaponStats().maxDashAmount; 
        maxWeakDashAmount = thisWeapon.GetWeaponStats().maxWeakDashAmount;
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
        Vector2 forward = new Vector2(transform.up.x, transform.up.y);
        if (Vector2.Dot(moveDirection.normalized, forward) > 1.1f) return;

        if (context.started && !isDashing && (Mathf.Abs(dashAmount) < maxWeakDashAmount)) 
        {
            isDashing = true;
            dashTime = dashDuration; // Set the dash duration
            cooldownTime = dashCooldown; // Reset the cooldown
            dashDirection = moveDirection;
            dashAmount--;

            if (dashAmount > 0)
            {
                ApplyInvincibility(dashDuration + (dashAmount * 0.2f));
            }
        }
    }

    private void ApplyInvincibility(float amount)
    {
        if (thisHealth == null) return;

        thisHealth.SetInvincibilityTime(amount, false);
    }

    public UiInfoStore GetInfo()
    {
        UiInfoStore infoStore = new UiInfoStore();
        infoStore.SetInfo(UiInfoType.Dashes, dashAmount);
        infoStore.SetInfoLock(UiInfoType.Dashes, true);

        return infoStore;
    }
}