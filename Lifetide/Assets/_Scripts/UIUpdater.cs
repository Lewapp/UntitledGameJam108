using TMPro;
using UnityEngine;
using System.Collections.Generic;
using static UiInfoStore;

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
            UiInfoStore thisInfoStore = uiReadings[i].GetInfo();

            // Check and update health display
            if (thisInfoStore.CheckInfoLock(UiInfoType.Health))
            {
                thisInfoStore.TryGetInfo(UiInfoType.Health, out float health);
                healthTXT.text = "Health: " + health;
            }

            // Check and update shield display
            if (thisInfoStore.CheckInfoLock(UiInfoType.Shield))
            {
                thisInfoStore.TryGetInfo(UiInfoType.Shield, out int shield);
                shieldTXT.text = "Shield: " + shield;
            }

            // Check and update dashes display
            if (thisInfoStore.CheckInfoLock(UiInfoType.Dashes))
            {
                thisInfoStore.TryGetInfo(UiInfoType.Dashes, out float dashes);
                dashesTXT.text = "Dashes: " + dashes;
            }
        }
    }
}
