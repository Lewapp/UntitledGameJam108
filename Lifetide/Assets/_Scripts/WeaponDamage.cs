using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class WeaponDamage : MonoBehaviour
{
    public float damage;

    private IWeaponStatusable weaponInfo;
    private List<GameObject> hitEnemies;

    public void Start()
    {
        hitEnemies = new List<GameObject>();

        foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
        {
            if (script != this)
            {
                IWeaponStatusable status = script.GetComponent<IWeaponStatusable>();
                if (status != null)
                {
                    weaponInfo = status;
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (weaponInfo != null)
        {
            if (!weaponInfo.IsAttacking() && hitEnemies.Count > 0)
            {
                hitEnemies = new List<GameObject>();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hitEnemies.Contains(collision.gameObject)) return;

        if (weaponInfo.IsAttacking())
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                hitEnemies.Add(collision.gameObject);
            }
        }
    }


}
