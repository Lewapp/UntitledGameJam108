using UnityEngine;
using static UiInfoStore;
using static Unity.VisualScripting.Member;

public class Health : MonoBehaviour, IDamageable, IUiReadable
{
    public int a;
    public int shieldAmount { get; set; }

    public CharacterStats characterStats;
    public float currentHealth;

    private float iTime = 0f;
    private float shieldTime = 0f;
    private IWeaponReadable weaponInfo;
    private WeaponStats weaponStats;


    private void Start()
    {
        currentHealth = characterStats.maxHealth;

        weaponInfo = GetComponent<IWeaponReadable>();

        if (weaponInfo?.weaponStatus != null)
            weaponStats = weaponInfo.weaponStatus.GetWeaponStats();

        if (weaponStats) shieldAmount = weaponStats.blocking;
    }

    private void Update()
    {
        a = shieldAmount;
        if (iTime > 0)
        {
            iTime -= Time.deltaTime;
        }

        ShieldRecharge();

        if (shieldAmount > 0 && weaponInfo?.weaponStatus != null)
        {
            weaponInfo.weaponStatus.CanBlock = true;
        }
    }

    public void TakeDamage(float amount, GameObject source)
    {
        if (iTime > 0)
            return;

        if (!ShieldCheck(source.transform))
        currentHealth -= amount;

        HealthCheck();
    }

    private void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private bool ShieldCheck(Transform source)
    {
        if (weaponInfo?.weaponStatus?.IsBlocking == true)
        {
            shieldTime = 0f;
            shieldAmount--;

            float blockRange = 1 - CheckBlockingPotential(source);

            if (blockRange > weaponStats.blockRange)
            {
                shieldAmount = 0;
            }

            if (shieldAmount <= 0)
                weaponInfo.weaponStatus.CanBlock = false;

            return true;
        }

        return false;
    }

    private float CheckBlockingPotential(Transform source)
    {
        Vector2 direction = (source.position - transform.position).normalized;

        float staringAmount = Vector2.Dot(transform.up, direction);
        staringAmount = Mathf.Clamp01((staringAmount + 1f) / 2);

        return staringAmount;
    }


    private void ShieldRecharge()
    {
        if (shieldAmount < weaponStats?.blocking)
        {
            shieldTime += Time.deltaTime;

            if (shieldTime >= weaponStats.shieldRechargeRate.y && shieldAmount > 0)
            {
                shieldTime = 0f;
                shieldAmount++;
            }
            else if (shieldTime >= weaponStats.shieldRechargeRate.x)
            {
                shieldTime = 0f;
                shieldAmount++;
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void AddInvincibilityTime(float amount)
    {
        iTime += amount;
    }

    public void SetInvincibilityTime(float amount, bool force)
    {
        if (!force)
        {
            iTime = (iTime > amount) ? iTime : amount;
            return;
        }
        iTime = amount;
    }

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
