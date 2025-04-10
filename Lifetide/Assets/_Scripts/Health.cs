using UnityEngine;
using static UiInfoStore;

public class Health : MonoBehaviour, IDamageable, IUiReadable
{
    public CharacterStats characterStats;
    public float currentHealth;

    private float iTime = 0f;

    private void Start()
    {
        currentHealth = characterStats.maxHealth;
    }

    private void Update()
    {
        if (iTime > 0)
        {
            iTime -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount)
    {
        if (!(iTime > 0))
        {
            currentHealth -= amount;
        }
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public UiInfoStore GetInfo()
    {
        UiInfoStore infoStore = new UiInfoStore();
        infoStore.SetInfo(UiInfoType.Health, currentHealth);
        infoStore.SetInfoLock(UiInfoType.Health, true);

        return infoStore;
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
}
