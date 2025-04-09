using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    public GameObject[] weapons;
    public float moveSpeed = 1.0f;

    private List<IEnemyUseable> weaponUses = new List<IEnemyUseable>();
    private List<IWeaponStatusable> weaponInfo = new List<IWeaponStatusable>();
    private bool playerInRange = false;
    private bool attackActive = false;

    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); } // Reference to Rigidbody2D

    private void Start()
    {
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
        if (playerInRange && weaponUses != null)
        {
            if (!attackActive) StartCoroutine(AttackDelay(0.1f));
        }
        else 
        {
            foreach (IWeaponStatusable weaponI in weaponInfo)
            {
                if (weaponI == null) continue;
                if (!weaponI.IsAttacking())
                {
                    rb.linearVelocity = new Vector2(transform.up.x * moveSpeed, transform.up.y * moveSpeed);
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

            weaponU.EnemyAttack();
        }
        attackActive = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}
