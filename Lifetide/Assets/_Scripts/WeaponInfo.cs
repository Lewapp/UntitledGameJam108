using UnityEngine;

public class WeaponInfo : MonoBehaviour, IWeaponReadable
{
    public GameObject currentWeapon;

    public GameObject weapon { get; set; }
    public IWeaponStatusable weaponStatus { get; set; }

    private void Awake()
    {
        UpdateWeaponInfo();
    }

    private void Update()
    {
        UpdateWeaponInfo();
    }

    private void UpdateWeaponInfo()
    {
        if (currentWeapon)
        {
            weapon = currentWeapon;
            if (weaponStatus == null)
            {
                weaponStatus = weapon.GetComponent<IWeaponStatusable>();
            }
        }
    }
}
