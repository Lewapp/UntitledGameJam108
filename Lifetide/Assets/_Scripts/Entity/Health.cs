using Unity.VisualScripting;
using UnityEngine;
using static InfoStore;
using static Unity.VisualScripting.Member;

public class Health : MonoBehaviour, IDamageable, IUiReadable, IDirectable
{
    public int shieldAmount { get; set; }    // Current number of shield points available for blocking
    public float mass { get; set; }
    public bool difficultyApplied { get; set; }    // States whether difficulty has already been applied or not

    public CharacterStats.characterTypes characterType;    // Reference to character's base stats
    public GameObject hitSpawn;
    public GameObject deathSound;
    public GameObject hitSound;
    public GameObject armourSound;
    public float currentHealth;              // Current health value of the character

    private IWeaponReadable weaponInfo;      // Cached weapon information 
    private WeaponStats weaponStats;         // Cached weapon stats
    private float iTime = 0f;                // Invincibility timer (prevents repeated damage in short time)
    private float shieldTime = 0f;           // Timer tracking how long since the last shield recharge
    private float maxHealth;

    private void Awake()
    {
        SpawnDirector.Register(this, gameObject);
    }

    private void OnDestroy()
    {
        SpawnDirector.UnRegister(this, gameObject);
    }

    private void Start()
    {
        // Get weapon info and weapon stats if available
        weaponInfo = GetComponent<IWeaponReadable>();
        if (weaponInfo?.weaponStatus != null)
            weaponStats = weaponInfo.weaponStatus[0].GetWeaponStats();

        // If weapon has shielding capability, initialise shield amount
        if (weaponStats) shieldAmount = weaponStats.blocking;

        currentHealth = maxHealth;
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
            weaponInfo.weaponStatus[0].CanBlock = true;
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
        {
            if (hitSound)
                Instantiate(hitSound, transform.position, Quaternion.identity);

            currentHealth -= amount;
            if (amount > 0 && hitSpawn)
            {
                Instantiate(hitSpawn, transform.position, Quaternion.identity);
            }
        }
        else
        {
            if (armourSound)
                Instantiate(armourSound, transform.position, Quaternion.identity);
        }
            HealthCheck();

        IWeaponReadable weaponReadable = source.transform.parent.GetComponent<IWeaponReadable>();
        IDamageable sourceDamagable = source.transform.parent.GetComponent<IDamageable>();
        if (weaponReadable != null)
        {
            foreach (var weapon in weaponReadable.weaponStatus)
                sourceDamagable.Heal(weapon.GetWeaponStats().healthRegen, source);
        }
    }

    public void Heal(float amount, GameObject source)
    {
        currentHealth += amount;

        HealthCheck();
    }

    /// <summary>
    /// Checks if health has dropped to zero or below,
    /// and handles character death.
    /// </summary>
    private void HealthCheck()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (currentHealth < 1)
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
        if (weaponInfo?.weaponStatus[0]?.IsBlocking == true)
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
            {
                for (int i = 0; i < weaponInfo.weaponStatus.Count; i++)
                {
                   weaponInfo.weaponStatus[i].CanBlock = false;
                }
            }


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
        Vector2 sourcePosition = new Vector2(source.position.x, source.position.y);
        Vector2 thisPosition = new Vector2(transform.position.x, transform.position.y);

        Vector2 direction = (sourcePosition - thisPosition).normalized;

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
        if (characterType != CharacterStats.characterTypes.Player && GlobalData.playerInfo)
        {
            GlobalData.playerInfo.kills++;
            if (characterType != CharacterStats.characterTypes.Basic)
            {
                GlobalData.playerInfo.specialsKilled++;
            }
        }

        if (deathSound)
            Instantiate(deathSound, transform.position, Quaternion.identity);

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
    public InfoStore GetInfo()
    {
        InfoStore infoStore = new InfoStore();
        infoStore.SetInfo(InfoType.Health, currentHealth);
        infoStore.SetInfoLock(InfoType.Health, true);

        infoStore.SetInfo(InfoType.Shield, shieldAmount);
        infoStore.SetInfoLock(InfoType.Shield, true);

        return infoStore;
    }

    public void SetDifficulty(DifficultyInfo difficultyInfo)
    {
        if (!difficultyInfo)
            return;

        for (int i = 0; i < difficultyInfo.characterStats.Count; i++)
        {
            if (difficultyInfo.characterStats[i].characterType != characterType)
            {
                continue;
            }

            foreach (CharacterStats.CharacterDifficultyStats cds in difficultyInfo.characterStats[i].stats)
            {
                if (cds.difficulty == difficultyInfo.difficultyType)
                {
                    maxHealth = cds.maxHealth;
                    currentHealth = maxHealth;
                    mass = cds.mass;
                }
            }
        }

        difficultyApplied = true;
    }

    public CharacterStats.characterTypes GetCharacterType()
    {
        return characterType;
    }

    public void Activate() { }
}