using UnityEngine;
using UnityEngine.UI;

public class WeaponSelection : MonoBehaviour
{
    #region Properties and References

    public PlayerInfo playerInfo;
    public PlayerInfo.WeaponTypes weaponType;
    public Vector2 positionOff;
    public Vector2 positionOn;

    [Header("Panels & Colours")]
    public Image weaponPanel;
    public Image textPanel;
    public Color weaponPanelOff;
    public Color weaponPanelOn;
    public Color textPanelOff;
    public Color textPanelOn;

    private RectTransform rect { get => GetComponent<RectTransform>(); }
    #endregion

    private void Start()
    {
        playerInfo.selectedWeapon = PlayerInfo.WeaponTypes.Sword;
    }

    private void Update()
    {
        if (playerInfo?.selectedWeapon == weaponType)
        {
            weaponPanel.color = weaponPanelOn;
            textPanel.color = textPanelOn;

            rect.anchoredPosition = positionOn;
        }
        else
        {
            weaponPanel.color = weaponPanelOff;
            textPanel.color = textPanelOff;

            rect.anchoredPosition = positionOff;
        }
    }

    public void SelectWeapon()
    {
        playerInfo.selectedWeapon = weaponType;
    }
}
