using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float amount);
    public void AddInvincibilityTime(float amount);
    public void SetInvincibilityTime(float amount, bool force);
}
