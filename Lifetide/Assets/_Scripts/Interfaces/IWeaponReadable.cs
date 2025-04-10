using UnityEngine;

public interface IWeaponReadable
{
    public GameObject weapon { get; set;  }
    public IWeaponStatusable weaponStatus { get; set; }
}
