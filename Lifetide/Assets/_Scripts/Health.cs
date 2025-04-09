using UnityEngine;

public class Health : MonoBehaviour, IDamageable
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
}
