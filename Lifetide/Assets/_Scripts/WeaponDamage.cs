using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]  
public class WeaponDamage : MonoBehaviour
{
    // The amount of damage dealt by the weapon.
    public float damage;

    // Reference to the weapon's status interface (used to check if the weapon is attacking).
    private IWeaponStatusable weaponInfo;

    // List of enemies that have been hit by the weapon to avoid hitting them multiple times.
    public List<GameObject> hitEnemies;

    public void Start()
    {
        // Initialise the list of enemies that the weapon has already hit.
        hitEnemies = new List<GameObject>();

        IWeaponStatusable status = gameObject.GetComponent<IWeaponStatusable>();
        // If a weapon status interface is found, assign it and break the loop.
        if (status != null) weaponInfo = status;
    }

    private void Update()
    {
        // If weaponInfo is set and the weapon is not attacking, clear the list of hit enemies.
        if (weaponInfo != null)
        {
            if (!weaponInfo.IsAttacking() && hitEnemies.Count > 0)
            {
                hitEnemies = new List<GameObject>();  // Reset the list of enemies that have been hit.
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the enemy has already been hit, do not apply damage again.
        if (hitEnemies.Contains(collision.gameObject)) return;

        // Check if the weapon is currently attacking.
        if (weaponInfo.IsAttacking())
        {
            // Try to find a damageable component (i.e., an enemy) in the collided object.
            IDamageable damageable = collision.GetComponent<IDamageable>();

            // If the collided object is damageable, apply damage and add it to the hit list.
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                hitEnemies.Add(collision.gameObject);  // Add the enemy to the list to prevent multiple hits.
            }
        }
    }
}