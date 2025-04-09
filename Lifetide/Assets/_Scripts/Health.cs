using UnityEngine;
using static UiInfoStore;

public class Health : MonoBehaviour, IDamageable, IUiReadable
{
    public CharacterStats characterStats;
    public float currentHealth;

    private void Start()
    {
        currentHealth = characterStats.maxHealth;
    }

    public void TakeDamage(float amount)
    {
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
}
