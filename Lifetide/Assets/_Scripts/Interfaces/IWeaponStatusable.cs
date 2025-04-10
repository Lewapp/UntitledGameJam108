using UnityEngine;

public interface IWeaponStatusable
{
    public bool IsAttacking();
    public bool AttackStart { get; set; }
    public WeaponStats GetWeaponStats();
}
