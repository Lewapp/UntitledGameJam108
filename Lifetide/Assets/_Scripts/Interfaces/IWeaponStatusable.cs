using UnityEngine;

public interface IWeaponStatusable
{
    public bool AttackStart { get; set; }
    public bool IsBlocking { get; set; }
    public bool CanBlock { get; set; }

    public bool IsAttacking();
    public WeaponStats GetWeaponStats();

}
