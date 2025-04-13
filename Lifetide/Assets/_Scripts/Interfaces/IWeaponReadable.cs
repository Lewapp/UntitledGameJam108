using UnityEngine;
using System.Collections.Generic;

public interface IWeaponReadable
{
    public List<IWeaponStatusable> weaponStatus { get; set; }

    public List<GameObject> GetWeapons();
}
