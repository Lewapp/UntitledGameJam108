using UnityEngine;

public class DifficultyManagement : MonoBehaviour, IUiReadable
{
    public PlayerInfo playerInfo;
    public DifficultyInfo[] difficulties;
    public DifficultyInfo currentDifficulty;

    private void Awake()
    {
        foreach (DifficultyInfo difficulty in difficulties)
        {
            if (difficulty.difficultyType == playerInfo.selectedDifficulty)
            {
                currentDifficulty = difficulty;
                break;
            }
        }

    }

    public InfoStore GetInfo()
    {
        InfoStore infoStore = new InfoStore();
        infoStore.SetInfo(InfoStore.InfoType.Difficulty, currentDifficulty);
        infoStore.SetInfoLock(InfoStore.InfoType.Difficulty, true);

        return infoStore;
    }
}
