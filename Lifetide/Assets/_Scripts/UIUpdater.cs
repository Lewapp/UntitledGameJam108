using TMPro;
using UnityEngine;
using System.Collections.Generic;
using static UiInfoStore;

public class UIUpdater : MonoBehaviour
{
    public TextMeshProUGUI healthTXT;

    public GameObject player;

    private List<IUiReadable> uiReadings = new List<IUiReadable>();

    private void Start()
    {
        if (player)
        {
            IUiReadable playerUI = player.GetComponent<IUiReadable>();
            if (playerUI != null) uiReadings.Add(playerUI);
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
        }
    }
}
