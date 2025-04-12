using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private void Start()
    {
        UnPauseGame();
    }

    public void PauseGame()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void Restart()
    {
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
