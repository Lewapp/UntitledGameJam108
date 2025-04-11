using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour, IDirectable
{
    public bool difficultyApplied { get; set; }    // States whether difficulty has already been applied or not

    public CharacterStats.characterTypes enemyType;
    public GameObject[] weapons;            // Array of weapons the enemy can use
    public float moveSpeed = 1.0f;          // Movement speed of the enemy when approaching the player
    public float attackMovement = 0f;          // Movement speed of the enemy when approaching the player whilst attacking

    // Internal lists to store weapon behaviours and status info
    private List<IEnemyUseable> weaponUses = new List<IEnemyUseable>();
    private List<IWeaponStatusable> weaponInfo = new List<IWeaponStatusable>();
    private bool playerInRange = false;     // Tracks if the player is currently within range
    private bool attackActive = false;      // Used to prevent the enemy from spamming attacks too rapidly

    // Quick reference to the Rigidbody2D component
    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); }

    private void Awake()
    {
        SpawnDirector.Register(this, gameObject);
    }

    private void OnDestroy()
    {
        SpawnDirector.UnRegister(this, gameObject);
    }

    private void Start()
    {
        // Gather usable weapons and their interfaces at the start
        if (weapons.Length > 0)
        {
            foreach (GameObject weapon in weapons)
            {
                weaponUses.Add(weapon.GetComponent<IEnemyUseable>());
                weaponInfo.Add(weapon.GetComponent<IWeaponStatusable>());
            }
        }
    }

    private void Update()
    {
        // If the player is close enough and weapons exist, try to attack
        if (playerInRange && weaponUses != null)
        {
            if (!attackActive)
                StartCoroutine(AttackDelay(0.1f)); // Add a slight delay between attacks
        }
        else
        {
            // If not attacking, continue moving forward
            foreach (IWeaponStatusable weaponI in weaponInfo)
            {
                if (weaponI == null) continue;

                // Only move if the weapon is not mid-attack
                if (!weaponI.IsInAnimation())
                {
                    rb.linearVelocity = new Vector2(transform.up.x * moveSpeed, transform.up.y * moveSpeed);
                }
                else
                {
                    rb.linearVelocity = new Vector2(transform.up.x * attackMovement, transform.up.y * attackMovement);
                }
            }
        }
    }



    private IEnumerator AttackDelay(float amount)
    {
        attackActive = true;
        yield return new WaitForSeconds(amount);

        foreach (IEnemyUseable weaponU in weaponInfo)
        {
            if (weaponU == null) continue;

            // Perform the attack using the weapon's interface
            weaponU.EnemyAttack();
        }

        attackActive = false;
    }

    // Detects if the player stays within the enemy's trigger area
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Detects when the player leaves the enemy's trigger area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void SetDifficulty(DifficultyInfo difficultyInfo)
    {
        if (!difficultyInfo) return;

        for (int i = 0; i < difficultyInfo.characterStats.Count; i++)
        {
            if (difficultyInfo.characterStats[i].characterType != enemyType)
            {
                continue;
            }

            foreach (CharacterStats.CharacterDifficultyStats cds in difficultyInfo.characterStats[i].stats)
            {
                if (cds.difficulty == difficultyInfo.difficultyType)
                {
                    moveSpeed = cds.moveSpeed;
                    attackMovement = cds.attackMoveSpeed;
                    for (int w = 0; w < weaponInfo.Count; w++)
                    {
                        weaponInfo[w].AttackSpeedMultiplier = cds.attackSpeedMp;
                        weaponInfo[w].DelayMultiplier = cds.delayMp;          
                    }
                }
            }   
        }

        difficultyApplied = true;
    }
}