using UnityEngine;

public interface IWeaponStatusable
{
    public CharacterStats.characterTypes ParentType { get; set; }
    public bool AttackStart { get; set; }
    public bool IsBlocking { get; set; }
    public bool CanBlock { get; set; }

    public float AttackSpeedMultiplier { get; set; }
    public float DelayMultiplier { get; set; }

    public bool IsAttacking();
    public bool IsInAnimation();
    public WeaponStats GetWeaponStats();

}
