using UnityEngine;

public class UIShields : MonoBehaviour
{
    public GameObject player;

    private IWeaponStatusable weaponStatus;

    private void Start()
    {
        foreach (Transform child in player.transform)
        {
            weaponStatus = child.GetComponent<IWeaponStatusable>();
        }
  
    }

    private void Update()
    {
        if (weaponStatus != null) 
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(weaponStatus.IsBlocking);
            }
    }

}
