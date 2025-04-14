using UnityEngine;

public class UIShields : MonoBehaviour
{
    public GameObject player;

    private IWeaponStatusable weaponStatus;

    private void Update()
    {
        if (weaponStatus != null)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(weaponStatus.IsBlocking);
            }
            return;
        }

        foreach (Transform child in player.transform)
        {
            if (child.gameObject.activeSelf)
            {
                weaponStatus = child.GetComponent<IWeaponStatusable>();
                if (weaponStatus != null)
                    break;
            }

        }
    }

}
