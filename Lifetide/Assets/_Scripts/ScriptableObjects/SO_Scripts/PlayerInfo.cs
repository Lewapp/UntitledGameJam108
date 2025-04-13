using UnityEngine;

[CreateAssetMenu(fileName = "Player Info", menuName = "ScriptableObjects/PlayerInfo")]
public class PlayerInfo : ScriptableObject
{
    public WeaponTypes selectedWeapon;
    public DifficultyInfo.Difficulties selectedDifficulty;

    [Header("Preferences")]
    public float musicVolume;
    public float sfxVolume;

    [Header("Player Performance")]
    public float timeSurvived;
    public int kills;
    public int specialsKilled;

    public enum WeaponTypes
    {
        Sword,
        Axe,
        Knife,
    }
}
