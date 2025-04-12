using TMPro;
using UnityEngine;
using System.Collections.Generic;
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
        }
    }

    private void Update()
    {
        for (int i = 0; i < uiReadings.Count; i++)
        {
            // Retrieve the information store for this UI-readable component
            InfoStore thisInfoStore = uiReadings[i].GetInfo();

            // Check and update health display
            if (thisInfoStore.CheckInfoLock(InfoType.Health))
            {
                thisInfoStore.TryGetInfo(InfoType.Health, out float health);
                healthTXT.text = "Health: " + (int)health;
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
    }
}
