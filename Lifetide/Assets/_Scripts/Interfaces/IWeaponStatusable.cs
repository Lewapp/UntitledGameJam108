using UnityEngine;

public interface IWeaponStatusable
{
    public CharacterStats.characterTypes ParentType { get; set; }
    public bool AttackStart { get; set; }
    public bool IsBlocking { get; set; }
    public bool CanBlock { get; set; }
    public int attackNo { get; set; }
    public float damageScale { get; set; }
    public float penScale { get; set; }

    public float AttackSpeedMultiplier { get; set; }
    public float DelayMultiplier { get; set; }

    public bool IsAttacking();
    public bool IsInAnimation();
    public WeaponStats GetWeaponStats();
    public PlayerInfo.WeaponTypes GetWeaponType();

}
