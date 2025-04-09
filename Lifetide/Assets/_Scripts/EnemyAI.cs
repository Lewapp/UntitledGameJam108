using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAI : MonoBehaviour
{
    public GameObject weapon;
    public float moveSpeed = 1.0f;

    private IEnemyUseable weaponUses;
    private IWeaponStatusable weaponInfo;
    private bool playerInRange = false;
    private bool attackActive = false;

    private Rigidbody2D rb { get => GetComponent<Rigidbody2D>(); } // Reference to Rigidbody2D

    private void Start()
    {
        if (weapon)
        {
            weaponUses = weapon.GetComponent<IEnemyUseable>();
            weaponInfo = weapon.GetComponent<IWeaponStatusable>();
        }
    }

    private void Update()
    {
        if (playerInRange && weaponUses != null)
        {
            if (!attackActive) StartCoroutine(AttackDelay(0.1f));
        }
        else if (!weaponInfo.IsAttacking())
        {
            rb.linearVelocity = new Vector2(transform.up.x * moveSpeed, transform.up.y * moveSpeed);
        }
    }

    private IEnumerator AttackDelay(float amount)
    {
        attackActive = true;
        yield return new WaitForSeconds(amount);
        weaponUses.EnemyAttack();
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
