using UnityEngine;

public interface IDamageable
{
    public int shieldAmount { get; set; }

    public void TakeDamage(float amount, GameObject source);
    public void AddInvincibilityTime(float amount);
    public void SetInvincibilityTime(float amount, bool force);
}
