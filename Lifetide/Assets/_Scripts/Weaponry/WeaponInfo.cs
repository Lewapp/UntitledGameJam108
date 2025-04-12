using UnityEngine;

public class WeaponInfo : MonoBehaviour, IWeaponReadable
{
    #region Properties and References

    public IWeaponStatusable weaponStatus { get; set; }     // Interface reference for accessing weapon status functionality
    public GameObject weapon { get; set; }                  // The weapon being tracked, with public getter and setter
    public PlayerInfo playerInfo;
    public GameObject[] weapons;
    public GameObject currentWeapon;                        // The currently equipped weapon GameObject, assigned via the Inspector


    #endregion

    private void Awake()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
            IWeaponStatusable weaponStatus = weapon.GetComponent<IWeaponStatusable>();
            if (weaponStatus == null)
                continue;

            if (playerInfo.selectedWeapon == weaponStatus.GetWeaponType())
            {
                weapon.SetActive(true);
                currentWeapon = weapon;
            }

        }

        UpdateWeaponInfo();
    }

    private void Update()
    {
        UpdateWeaponInfo();
    }

    // Updates the internal weapon information based on the currentWeapon reference
    private void UpdateWeaponInfo()
    {
        if (currentWeapon)
        {
            // Assigns the current weapon to the internal weapon reference
            weapon = currentWeapon;

            // Retrieves the IWeaponStatusable component only if not already set
            if (weaponStatus == null)
            {
                weaponStatus = weapon.GetComponent<IWeaponStatusable>();
            }
        }
    }
}
