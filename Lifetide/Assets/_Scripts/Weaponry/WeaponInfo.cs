using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WeaponInfo : MonoBehaviour, IWeaponReadable
{
    #region Properties and References

    public List<IWeaponStatusable> weaponStatus { get; set; }     // Interface reference for accessing weapon status functionality
    public List<GameObject> weapons { get; set; }                  // The weapon being tracked
    public List<GameObject> availableWeapons;
    public List<GameObject> currentWeapons;                        // The currently equipped weapon GameObject, assigned via the Inspector


    #endregion

    //TODO make all weapon variables accept multiple weapons

    private void Awake()
    {
        weaponStatus = new List<IWeaponStatusable>();
        weapons = new List<GameObject>();
        currentWeapons = new List<GameObject>();
        foreach (GameObject weapon in availableWeapons)
        {
            weapon.SetActive(false);
            IWeaponStatusable wStatus = weapon.GetComponent<IWeaponStatusable>();
            if (wStatus == null)
                continue;
            if (GlobalData.playerInfo.selectedWeapon == wStatus.GetWeaponType())
            {
                weapon.SetActive(true);
                currentWeapons.Add(weapon);
                weaponStatus.Add(wStatus);
            }
        }
    }

    public List<GameObject> GetWeapons()
    {
        return currentWeapons;
    }
}
