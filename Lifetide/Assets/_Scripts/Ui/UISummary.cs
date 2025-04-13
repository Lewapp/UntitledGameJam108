using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISummary : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public TextMeshProUGUI timeValue;
    public TextMeshProUGUI killsValue;
    public TextMeshProUGUI specialsValue;

    private void Start()
    {
        int timeSurvived = (int)playerInfo.timeSurvived;

        timeValue.text = timeSurvived.ToString();
        killsValue.text = playerInfo.kills.ToString();
        specialsValue.text = playerInfo.specialsKilled.ToString();

    }

}
