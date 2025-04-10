using TMPro;
using UnityEngine;
using System.Collections.Generic;
using static UiInfoStore;

public class UIUpdater : MonoBehaviour
{
    public TextMeshProUGUI healthTXT;
    public TextMeshProUGUI shieldTXT;
    public TextMeshProUGUI dashesTXT;

    public GameObject player;

    private List<IUiReadable> uiReadings = new List<IUiReadable>();

    private void Start()
    {
        if (player)
        {
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
            UiInfoStore thisInfoStore = uiReadings[i].GetInfo();
            if (thisInfoStore.CheckInfoLock(UiInfoType.Health))
            {
                thisInfoStore.TryGetInfo(UiInfoType.Health, out float health);
                healthTXT.text = "Health: " + health;
            }
            if (thisInfoStore.CheckInfoLock(UiInfoType.Shield))
            {
                thisInfoStore.TryGetInfo(UiInfoType.Shield, out int shield);
                shieldTXT.text = "Shield: " + shield;
            }
            if (thisInfoStore.CheckInfoLock(UiInfoType.Dashes))
            {
                thisInfoStore.TryGetInfo(UiInfoType.Dashes, out float dashes);
                dashesTXT.text = "Dashes: " + dashes;
            }
        }
    }
}
