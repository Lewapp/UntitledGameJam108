using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public PlayerInfo playerInfo;
    public TextMeshProUGUI difficultyText;

    private DifficultyInfo.Difficulties currentDifficulty;

    private void Awake()
    {
        GlobalData.playerInfo = playerInfo;
    }

    private void Start()
    {
        currentDifficulty = DifficultyInfo.Difficulties.Medium;
    }

    private void Update()
    {
        if (playerInfo.selectedDifficulty != currentDifficulty)
        {
            playerInfo.selectedDifficulty = currentDifficulty;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.LogWarning("Exit Game");
    }

    public void ChangeDifficulty()
    {
        switch (playerInfo.selectedDifficulty)
        {
            case DifficultyInfo.Difficulties.Easy:
                currentDifficulty = DifficultyInfo.Difficulties.Medium;
                break;
            case DifficultyInfo.Difficulties.Medium:
                currentDifficulty = DifficultyInfo.Difficulties.Hard;
                break;
            case DifficultyInfo.Difficulties.Hard:
                currentDifficulty = DifficultyInfo.Difficulties.Easy;
                break;
        }

        difficultyText.text = currentDifficulty.ToString();
    }
}
