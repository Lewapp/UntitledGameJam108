using System.Collections.Generic;
using UnityEngine;

public class HealingRing : MonoBehaviour
{
    public GameObject self;
    public GameObject healingParticle;
    public GameObject soundEffect;
    public string targetTag;

    public float healingAmount;
    public float healingRate;

    private List<GameObject> recentlyHealed;
    private float timeSinceLastHeal;

    private void Start()
    {
        recentlyHealed = new List<GameObject>();
    }

    private void Update()
    {
        timeSinceLastHeal += Time.deltaTime;

        if (timeSinceLastHeal >= healingRate)
        {
            timeSinceLastHeal = 0;
            recentlyHealed = new List<GameObject>();

            if (soundEffect)
                Instantiate(soundEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (recentlyHealed.Contains(collision.gameObject))
            return;

        if (collision.CompareTag(targetTag) && collision.gameObject != self)
        {
            recentlyHealed.Add(collision.gameObject);

            IDamageable damageable = collision.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.Heal(healingAmount, self);
                if (healingParticle)
                    Instantiate(healingParticle, collision.transform.position, Quaternion.identity);
            }
        }
    }
}
