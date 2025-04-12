using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static InfoStore;


public class UIUpdater : MonoBehaviour
{
    #region Properties and References

    // Text elements assigned via the Inspector for displaying player stats
    public TextMeshProUGUI healthTXT;
    public TextMeshProUGUI shieldTXT;
    public TextMeshProUGUI dashesTXT;
    // Reference to the player GameObject whose stats will be tracked
    public GameObject player;


    // A list of components implementing IUiReadable, used to pull UI-relevant data
    private List<IUiReadable> uiReadings = new List<IUiReadable>();

    #endregion

    private void Start()
    {
        if (player)
        {
            // Gather all components on the player that implement IUiReadable
            foreach (IUiReadable playerUI in player.GetComponents<IUiReadable>())
            {
                uiReadings.Add(playerUI);
            }
            foreach (Transform child in transform)
            {
                IUiReadable playerUI = child.GetComponent<IUiReadable>();

                if (playerUI != null)
                    uiReadings.Add(playerUI);
            }
        }

        StartCoroutine(UiUpdate());
    }

    private IEnumerator UiUpdate()
    {
        yield return null; 

        for (int i = 0; i < uiReadings.Count; i++)
        {
            // Retrieve the information store for this UI-readable component
            InfoStore thisInfoStore = uiReadings[i].GetInfo();
            if (thisInfoStore == null)
                continue;

            // Check and update health display
            if (thisInfoStore.CheckInfoLock(InfoType.Health))
            {
                thisInfoStore.TryGetInfo(InfoType.Health, out float health);
                healthTXT.text = "Health: " + (int)Mathf.Clamp(health, 0f, Mathf.Infinity);
                if (health <= 0)
                {
                    for (int x = 0; x < uiReadings.Count; x++)
                    {
                        uiReadings[x].Activate();
                    }
                }
            }

            // Check and update shield display
            if (thisInfoStore.CheckInfoLock(InfoType.Shield))
            {
                thisInfoStore.TryGetInfo(InfoType.Shield, out int shield);
                shieldTXT.text = "Shield: " + shield;
            }

            // Check and update dashes display
            if (thisInfoStore.CheckInfoLock(InfoType.Dashes))
            {
                thisInfoStore.TryGetInfo(InfoType.Dashes, out float dashes);
                dashesTXT.text = "Dashes: " + (int)dashes;
            }
        }

        StartCoroutine(UiUpdate());
    }  
}
