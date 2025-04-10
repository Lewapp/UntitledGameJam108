using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "ScriptableObjects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string weaponName;
    public float maxDamage;
    public float penPower;
    public int blocking;

    [Header("Movement")]
    public float moveSpeed = 15f; // Normal movement speed multiplier
    public float dashSpeed = 40f; // Dash speed multiplier
    public float weakDashNerf = 0.3f; // Weak dash speed multiplier per extra dash
    public float dashDuration = 0.1f; // Duration of dash
    public float dashCooldown = 2.5f; // Cooldown time between dashes
    public float maxDashAmount = 3f; // Amount of dashes within 1 cooldown allowed
    public float maxWeakDashAmount = 4f; // Amount of dashes within 1 cooldown allowed;
}
