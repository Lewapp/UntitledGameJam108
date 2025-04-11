using UnityEngine;

public class DifficultyManagement : MonoBehaviour, IUiReadable
{
    public DifficultyInfo[] difficulties;
    public DifficultyInfo currentDifficulty;

    public InfoStore GetInfo()
    {
        InfoStore infoStore = new InfoStore();
        infoStore.SetInfo(InfoStore.InfoType.Difficulty, currentDifficulty);
        infoStore.SetInfoLock(InfoStore.InfoType.Difficulty, true);

        return infoStore;
    }
}
