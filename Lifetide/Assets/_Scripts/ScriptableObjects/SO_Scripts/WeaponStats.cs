using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string weaponName;            // The name of the weapons
    public float maxDamage;              //  The max damage the weapon can deal
    public float healthRegen;            // Percentage of health restored per kill
    public float penPower;               // The amount of penetration power the weapon has
    public int blocking;                 // How many blocks the weapon has 
    public float blockRange;             // From 0 -> 1 where 1 blocks damage at 360 degrees
    public Vector2 shieldRechargeRate;   // X =  amount the first shield takes to recharge & Y = amount of time ithe rest takes to recharge 1 shield

    [Header("Movement")]
    public float moveSpeed = 15f;        // Normal movement speed multiplier
    public float shieldedMovement = 7f;  // Movement speed whilst shielded multiplier
    public float dashSpeed = 40f;        // Dash speed multiplier
    public float weakDashNerf = 0.3f;    // Weak dash speed multiplier per extra dash
    public float dashDuration = 0.1f;    // Duration of dash
    public float dashCooldown = 2.5f;    // Cooldown time between dashes
    public float maxDashAmount = 3f;     // Amount of dashes within 1 cooldown allowed
    public float maxWeakDashAmount = 4f; // Amount of weak dashes within 1 cooldown allowed
}
