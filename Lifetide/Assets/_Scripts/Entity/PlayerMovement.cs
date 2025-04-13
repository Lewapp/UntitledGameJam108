using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static InfoStore;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour, IUiReadable
{
    // TODO: Make weapons be a multiplier instead of a set stat for player so that player's CharacterStats are considered base stats
    public CharacterStats characterStats;

    [Header("Movement Settings")]
    public float moveSpeed;              // Normal movement speed 
    public float attackMovement;         // Attacking movement speed 
    public float shieldedMoveSpeed;      // Movement speed while blocking
    public float dashSpeed;              // Dash speed multiplier
    public float weakDashNerf;           // Speed reduction for weak dashes (after main dashes run out)
    public float dashDuration;           // Duration the dash lasts
    public float dashCooldown;           // Time before dashes replenish
    public float maxDashAmount;          // Max amount of strong dashes available before cooldown
    public float maxWeakDashAmount;      // Max total amount of weak + strong dashes allowed before cooldown

    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); } // Cached reference to Rigidbody2D
    private Vector2 moveDirection;       // Direction of player movement from input
    private Vector2 dashDirection;       // Stored direction for dash movement
    private bool isDashing = false;      // Whether the player is currently dashing
    private float dashTime = 0f;         // Current dash timer
    private float cooldownTime = 0f;     // Cooldown timer for dash replenishment
    private float dashAmount = 1f;       // Current number of available dashes

    private IWeaponReadable currrentWeapon;     // Interface for current weapon reference
    private IDamageable thisHealth;             // Interface for health and invincibility control
    private List<IWeaponStatusable> thisWeapon;       // Interface for weapon-specific stats and states

    private void Start()
    {
        thisWeapon = new List<IWeaponStatusable>();

        dashAmount = maxDashAmount;

        // Get components implementing damageable and weapon-readable interfaces
        thisHealth = GetComponent<IDamageable>();
        currrentWeapon = GetComponent<IWeaponReadable>();
        if (currrentWeapon != null)
        {
            for (int i = 0; i < currrentWeapon.GetWeapons().Count; i++)
            {
                thisWeapon.Add(currrentWeapon.GetWeapons()[i].GetComponent<IWeaponStatusable>());
            }
        }

        // Set movement stats based on current weapon or default
        SetMovementStats();
    }

    private void Update()
    {
        ApplyMovement();
        ApplyDash();
    }

    /// <summary>
    /// Applies dash movement each frame if dashing is active, including full or weakened dash speed based on remaining charges.
    /// Handles dash duration countdown and replenishes dash charges after the cooldown period ends.
    /// </summary>
    private void ApplyDash()
    {
        // Handle dash movement
        if (isDashing)
        {
            if (dashAmount > 0)
            {
                // Apply full dash speed
                rb.linearVelocity += dashDirection * dashSpeed;
            }
            else
            {
                // Apply weaker dash if strong dashes are depleted
                rb.linearVelocity += dashDirection * (dashSpeed - dashSpeed * weakDashNerf * Mathf.Abs(dashAmount));
            }

            // Reduce dash timer
            dashTime -= Time.deltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false; // End dash
            }
        }
        else
        {
            // Handle cooldown for restoring dash charges
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

    /// <summary>
    /// Applies current movement velocity based on direction and weapon/blocking status.
    /// </summary>
    private void ApplyMovement()
    {
        for (int i = 0; i < thisWeapon.Count; i++)
        {
            if (currrentWeapon?.weaponStatus[i]?.IsBlocking == true)
            {
                rb.linearVelocity = moveDirection * shieldedMoveSpeed;
            }
            else
            {
                rb.linearVelocity = moveDirection * moveSpeed;
                // Only move if the weapon is not mid-attack
                if (!thisWeapon[i].IsInAnimation())
                {
                    rb.linearVelocity = moveDirection * moveSpeed;
                }
                else
                {
                    rb.linearVelocity = moveDirection * attackMovement;
                }
            }
        }

    }

    /// <summary>
    /// Sets movement-related stats from the weapon or uses default values if unavailable.
    /// </summary>
    private void SetMovementStats()
    {
        if (thisWeapon == null || currrentWeapon == null)
        {
            Debug.LogWarning("Using default movement stats");
            moveSpeed = 15f;
            attackMovement = 15f;
            shieldedMoveSpeed = 7f;
            dashSpeed = 40f;
            weakDashNerf = 0.3f;
            dashDuration = 0.1f;
            dashCooldown = 2.5f;
            maxDashAmount = 3f;
            maxWeakDashAmount = 4f;
            return;
        }

        // Retrieve stats from weapon interface
        moveSpeed = thisWeapon[0].GetWeaponStats().moveSpeed;
        shieldedMoveSpeed = thisWeapon[0].GetWeaponStats().shieldedMovement;
        dashSpeed = thisWeapon[0].GetWeaponStats().dashSpeed;
        weakDashNerf = thisWeapon[0].GetWeaponStats().weakDashNerf;
        dashDuration = thisWeapon[0].GetWeaponStats().dashDuration;
        dashCooldown = thisWeapon[0].GetWeaponStats().dashCooldown;
        maxDashAmount = thisWeapon[0].GetWeaponStats().maxDashAmount;
        maxWeakDashAmount = thisWeapon[0].GetWeaponStats().maxWeakDashAmount;
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
    /// Called by the Input System when the dash action is triggered.
    /// Starts a dash if possible and handles dash logic.
    /// </summary>
    /// <param name="context">The input context for the dash action.</param>
    public void Dash(InputAction.CallbackContext context)
    {
        Vector2 forward = new Vector2(transform.up.x, transform.up.y);

        // Prevent dashing in a direction not aligned enough with forward
        if (Vector2.Dot(moveDirection.normalized, forward) > 1.1f) return;

        // Only allow dash if not currently dashing and max weak dashes not exceeded
        if (context.started && !isDashing && (Mathf.Abs(dashAmount) < maxWeakDashAmount))
        {
            isDashing = true;
            dashTime = dashDuration;           // Set dash duration
            cooldownTime = dashCooldown;       // Start cooldown timer
            dashDirection = moveDirection;     // Store dash direction
            dashAmount--;                      // Reduce dash count

            // Grant temporary invincibility during normal dashes
            if (dashAmount > 0)
            {
                ApplyInvincibility(dashDuration + (dashAmount * 0.2f));
            }
        }
    }

    /// <summary>
    /// Grants temporary invincibility [Title Card] to the player. 
    /// </summary>
    /// <param name="amount">Duration of invincibility.</param>
    private void ApplyInvincibility(float amount)
    {
        if (thisHealth == null) return;

        // Apply invincibility status via damageable interface
        thisHealth.SetInvincibilityTime(amount, false);
    }

    /// <summary>
    /// Returns dash-related UI information for display purposes.
    /// </summary>
    /// <returns>A UiInfoStore with current dash count info.</returns>
    public InfoStore GetInfo()
    {
        InfoStore infoStore = new InfoStore();

        // Set current dash count in UI
        infoStore.SetInfo(InfoType.Dashes, dashAmount);

        // Mark as unlocked to allow it to be read
        infoStore.SetInfoLock(InfoType.Dashes, true);

        return infoStore;
    }

    public void Activate() { }
}