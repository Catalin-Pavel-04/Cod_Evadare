using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectController2D : MonoBehaviour
{
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private Text[] levelButtonTexts;
    [SerializeField] private string[] levelSceneNames;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        RefreshButtons();
    }

    public void LoadLevel(int levelIndex)
    {
        if (!GameProgress2D.IsLevelUnlocked(levelIndex))
        {
            Debug.Log($"Level {levelIndex} is locked.", this);
            RefreshButtons();
            return;
        }

        int sceneIndex = levelIndex - 1;

        if (levelSceneNames == null || sceneIndex < 0 || sceneIndex >= levelSceneNames.Length || string.IsNullOrWhiteSpace(levelSceneNames[sceneIndex]))
        {
            Debug.LogWarning($"Cannot load level {levelIndex} because no scene name is configured.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSceneNames[sceneIndex]);
    }

    public void ReturnToMainMenu()
    {
        if (string.IsNullOrWhiteSpace(mainMenuSceneName))
        {
            Debug.LogWarning("Cannot return to main menu because mainMenuSceneName is empty.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void RefreshButtons()
    {
        if (levelButtons == null)
        {
            return;
        }

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            bool unlocked = GameProgress2D.IsLevelUnlocked(levelIndex);

            if (levelButtons[i] != null)
            {
                levelButtons[i].interactable = unlocked;
            }

            if (levelButtonTexts != null && i < levelButtonTexts.Length && levelButtonTexts[i] != null)
            {
                string sceneName = levelSceneNames != null && i < levelSceneNames.Length ? levelSceneNames[i] : string.Empty;
                levelButtonTexts[i].text = unlocked ? BuildLevelLabel(levelIndex, sceneName) : $"Level {levelIndex}: Locked";
            }
        }
    }

    private static string BuildLevelLabel(int levelIndex, string sceneName)
    {
        switch (levelIndex)
        {
            case 1:
                return "Level 1: Laboratory";
            case 2:
                return "Level 2: Prison";
            case 3:
                return "Level 3: Zombie City";
            case 4:
                return "Level 4: Sci-Fi Base";
            case 5:
                return "Level 5: Horror Hospital";
            default:
                return string.IsNullOrWhiteSpace(sceneName) ? $"Level {levelIndex}" : $"Level {levelIndex}: {sceneName}";
        }
    }
}
