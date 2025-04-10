using UnityEngine;
using static UiInfoStore;

public class Health : MonoBehaviour, IDamageable, IUiReadable
{
    public int shieldAmount { get; set; }    // Current number of shield points available for blocking

    public CharacterStats characterStats;    // Reference to character's base stats
    public float currentHealth;              // Current health value of the character

    private IWeaponReadable weaponInfo;      // Cached weapon information 
    private WeaponStats weaponStats;         // Cached weapon stats
    private float iTime = 0f;                // Invincibility timer (prevents repeated damage in short time)
    private float shieldTime = 0f;           // Timer tracking how long since the last shield recharge

    private void Start()
    {
        // Set current health to max at start
        currentHealth = characterStats.maxHealth;

        // Get weapon info and weapon stats if available
        weaponInfo = GetComponent<IWeaponReadable>();
        if (weaponInfo?.weaponStatus != null)
            weaponStats = weaponInfo.weaponStatus.GetWeaponStats();

        // If weapon has shielding capability, initialise shield amount
        if (weaponStats) shieldAmount = weaponStats.blocking;
    }

    private void Update()
    {
        // Count down invincibility time if active
        if (iTime > 0)
        {
            iTime -= Time.deltaTime;
        }

        // Attempt shield regeneration
        ShieldRecharge();

        // Enable blocking if shield is available
        if (shieldAmount > 0 && weaponInfo?.weaponStatus != null)
        {
            weaponInfo.weaponStatus.CanBlock = true;
        }
    }

    /// <summary>
    /// Applies damage to the character if not currently invincible.
    /// If blocking, damage may be absorbed by the shield instead.
    /// </summary>
    /// <param name="amount">Damage amount to apply</param>
    /// <param name="source">The object responsible for the damage</param>
    public void TakeDamage(float amount, GameObject source)
    {
        if (iTime > 0)
            return;

        // If shield fails, apply damage to health
        if (!ShieldCheck(source.transform))
            currentHealth -= amount;

        HealthCheck();
    }

    /// <summary>
    /// Checks if health has dropped to zero or below,
    /// and handles character death.
    /// </summary>
    private void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Checks if the character can block the incoming damage based on angle.
    /// Reduces shield amount if successfully blocked.
    /// </summary>
    /// <param name="source">The source of the attack</param>
    /// <returns>True if damage was blocked, false if not</returns>
    private bool ShieldCheck(Transform source)
    {
        if (weaponInfo?.weaponStatus?.IsBlocking == true)
        {
            shieldTime = 0f; // Reset recharge timer
            shieldAmount--;

            // Check if attack was within blocking angle
            float blockRange = 1 - CheckBlockingPotential(source);

            // If too far outside block range, shield breaks completely
            if (blockRange > weaponStats.blockRange)
            {
                shieldAmount = 0;
            }

            // If out of shield, disable blocking
            if (shieldAmount <= 0)
                weaponInfo.weaponStatus.CanBlock = false;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Calculates how directly the player is facing the source of the attack.
    /// Returns a value between 0 (not facing) and 1 (fully facing).
    /// </summary>
    private float CheckBlockingPotential(Transform source)
    {
        Vector2 direction = (source.position - transform.position).normalized;

        float staringAmount = Vector2.Dot(transform.up, direction);
        staringAmount = Mathf.Clamp01((staringAmount + 1f) / 2);

        return staringAmount;
    }

    /// <summary>
    /// Regenerates shield points over time based on weapon recharge rates.
    /// </summary>
    private void ShieldRecharge()
    {
        if (shieldAmount < weaponStats?.blocking)
        {
            shieldTime += Time.deltaTime;

            // If some shield remains, use a faster recharge rate
            if (shieldTime >= weaponStats.shieldRechargeRate.y && shieldAmount > 0)
            {
                shieldTime = 0f;
                shieldAmount++;
            }
            // If completely depleted, use slower recharge rate
            else if (shieldTime >= weaponStats.shieldRechargeRate.x)
            {
                shieldTime = 0f;
                shieldAmount++;
            }
        }
    }

    /// <summary>
    /// Handles death of the character by destroying the object.
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Adds a specific amount of invincibility time to prevent damage.
    /// </summary>
    public void AddInvincibilityTime(float amount)
    {
        iTime += amount;
    }

    /// <summary>
    /// Sets invincibility time, with the option to force override the current value.
    /// </summary>
    public void SetInvincibilityTime(float amount, bool force)
    {
        if (!force)
        {
            iTime = (iTime > amount) ? iTime : amount;
            return;
        }
        iTime = amount;
    }

    /// <summary>
    /// Returns current health and shield values for the UI system.
    /// Unlocks both values to ensure UI can read them
    /// </summary>
    public UiInfoStore GetInfo()
    {
        UiInfoStore infoStore = new UiInfoStore();
        infoStore.SetInfo(UiInfoType.Health, currentHealth);
        infoStore.SetInfoLock(UiInfoType.Health, true);

        infoStore.SetInfo(UiInfoType.Shield, shieldAmount);
        infoStore.SetInfoLock(UiInfoType.Shield, true);

        return infoStore;
    }
}