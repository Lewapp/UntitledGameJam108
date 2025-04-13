using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]  
public class WeaponDamage : MonoBehaviour, IDirectable
{
    public bool difficultyApplied { get; set ; }

    // Reference to the weapon's status interface (used to check if the weapon is attacking).
    private IWeaponStatusable weaponStatus;

    // List of enemies that have been hit by the weapon to avoid hitting them multiple times.
    public List<GameObject> hitEnemies;

    private float currentPenPower;
    private float damageMp;
    private float penMP;

    private void Awake()
    {
        IWeaponStatusable status = GetComponent<IWeaponStatusable>();
        // If a weapon status interface is found, assign it and break the loop.
        if (status != null) weaponStatus = status;
        SpawnDirector.Register(this, gameObject);
    }

    private void OnDestroy()
    {
        SpawnDirector.UnRegister(this, gameObject);
    }

    public void Start()
    {
        // Initialise the list of enemies that the weapon has already hit.
        hitEnemies = new List<GameObject>();
    }

    private void Update()
    {
        // If weaponInfo is set and the weapon is not attacking, clear the list of hit enemies.
        if (weaponStatus != null)
        {
            if (!weaponStatus.IsAttacking() && hitEnemies.Count > 0)
            {
                hitEnemies = new List<GameObject>();  // Reset the list of enemies that have been hit.
            }

            if (weaponStatus.AttackStart)
            {
                float penPower = weaponStatus.GetWeaponStats().penPower;
                penPower += penPower * (penMP + weaponStatus.penScale);

                currentPenPower = penPower;
                //  TODO: AttakNO seems to start at animation 1 not 0. Also, for some reason, I higher pen in the animation causes the damage to be lower. I thought it was doing animation 1 but wasnt
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool sameTeam = true;
        if (weaponStatus.ParentType == CharacterStats.characterTypes.Player)
        {
            if (collision.CompareTag("Enemy"))
                sameTeam = false;
        }
        else if (collision.CompareTag("Player"))
        {
            sameTeam = false;
        }

        if (collision.isTrigger || sameTeam) return;

        // If the enemy has already been hit, do not apply damage again.
        if (hitEnemies.Contains(collision.gameObject)) return;

        // Check if the weapon is currently attacking.
        if (weaponStatus.IsAttacking())
        {
            // Try to find a damageable component (i.e. an enemy) in the collided object.
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            // If the collided object is damageable, apply damage and add it to the hit list.
            if (damageable != null)
            {
                float maxPenPower = weaponStatus.GetWeaponStats().penPower;
                maxPenPower += maxPenPower * (penMP + weaponStatus.penScale);

                float powerPercent = currentPenPower / maxPenPower;

                float damage = weaponStatus.GetWeaponStats().maxDamage * Mathf.Clamp01(powerPercent);
                damage += (damage * damageMp) + (damage * weaponStatus.damageScale);
                damage = Mathf.Clamp(damage, 0, Mathf.Infinity);
                Debug.LogWarning(damage + " : " + currentPenPower + " : " + damageable.mass);
                damageable.TakeDamage(damage, gameObject);
                hitEnemies.Add(collision.gameObject);  // Add the enemy to the list to prevent multiple hits.

                currentPenPower -= damageable.mass;
                currentPenPower = Mathf.Clamp(currentPenPower, 0, Mathf.Infinity);
            }
        }
    }

    public void SetDifficulty(DifficultyInfo difficultyInfo)
    {
        if (!difficultyInfo)
            return;

        for (int i = 0; i < difficultyInfo.characterStats.Count; i++)
        {
            if (difficultyInfo.characterStats[i].characterType != weaponStatus.ParentType)
            {
                continue;
            }

            foreach (CharacterStats.CharacterDifficultyStats cds in difficultyInfo.characterStats[i].stats)
            {
                if (cds.difficulty == difficultyInfo.difficultyType)
                {
                    damageMp = cds.damageMp;
                    penMP = cds.penetrationMp;
                }
            }
            break;
        }
    }
}