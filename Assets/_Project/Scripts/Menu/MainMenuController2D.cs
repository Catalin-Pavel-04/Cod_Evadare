using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class MainMenuController2D : MonoBehaviour
{
    [SerializeField] private string[] campaignSceneNames =
    {
        "Level_01_Laboratory",
        "Level_02_Prison",
        "Level_03_ZombieCity",
        "Level_04_SciFiBase",
        "Level_05_HorrorHospital"
    };
    [SerializeField] private string[] campaignScenePaths =
    {
        "Assets/_Project/Scenes/Levels/Level_01_Laboratory.unity",
        "Assets/_Project/Scenes/Levels/Level_02_Prison.unity",
        "Assets/_Project/Scenes/Levels/Level_03_ZombieCity.unity",
        "Assets/_Project/Scenes/Levels/Level_04_SciFiBase.unity",
        "Assets/_Project/Scenes/Levels/Level_05_HorrorHospital.unity"
    };
    [SerializeField] private string levelSelectSceneName = "LevelSelect";
    [SerializeField] private string levelSelectScenePath = "Assets/_Project/Scenes/Game/LevelSelect.unity";
    [SerializeField] private string demoSceneName = "Prototype_FinalDemo";
    [SerializeField] private string demoScenePath = "Assets/_Project/Scenes/Prototype_FinalDemo.unity";
    [SerializeField] private string prisonSceneName = "Prototype_PrisonLevel";
    [SerializeField] private string prisonScenePath = "Assets/_Project/Scenes/Prototype_PrisonLevel.unity";
    [SerializeField] private string balancedPrisonSceneName = "Prototype_PrisonLevel_Balanced";
    [SerializeField] private string balancedPrisonScenePath = "Assets/_Project/Scenes/Prototype_PrisonLevel_Balanced.unity";
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        Time.timeScale = 1f;
        ShowMainMenu();
    }

    public void PlayDemo()
    {
        LoadScene(demoSceneName, demoScenePath);
    }

    public void PlayNewGame()
    {
        GameProgress2D.ResetProgress();
        LoadCampaignLevel(1);
    }

    public void ContinueGame()
    {
        LoadCampaignLevel(GameProgress2D.GetHighestUnlockedLevel());
    }

    public void ShowLevelSelect()
    {
        LoadScene(levelSelectSceneName, levelSelectScenePath);
    }

    public void PlayPrisonLevel()
    {
        LoadScene(prisonSceneName, prisonScenePath);
    }

    public void PlayBalancedPrisonLevel()
    {
        LoadScene(balancedPrisonSceneName, balancedPrisonScenePath);
    }

    public void LoadSceneByName(string sceneName)
    {
        LoadScene(sceneName, string.Empty);
    }

    private void LoadCampaignLevel(int levelIndex)
    {
        int safeLevelIndex = Mathf.Clamp(levelIndex, 1, campaignSceneNames != null && campaignSceneNames.Length > 0 ? campaignSceneNames.Length : 1);
        int arrayIndex = safeLevelIndex - 1;
        string sceneName = campaignSceneNames != null && arrayIndex < campaignSceneNames.Length ? campaignSceneNames[arrayIndex] : string.Empty;
        string scenePath = campaignScenePaths != null && arrayIndex < campaignScenePaths.Length ? campaignScenePaths[arrayIndex] : string.Empty;
        LoadScene(sceneName, scenePath);
    }

    private void LoadScene(string sceneName, string scenePath)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("Cannot load scene because the scene name is empty.", this);
            return;
        }

        Time.timeScale = 1f;

        if (IsSceneInBuildSettings(sceneName, scenePath))
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

#if UNITY_EDITOR
        if (!string.IsNullOrWhiteSpace(scenePath) && File.Exists(scenePath))
        {
            EditorSceneManager.LoadSceneInPlayMode(scenePath, new LoadSceneParameters(LoadSceneMode.Single));
            return;
        }
#endif

        Debug.LogWarning(
            $"Scene '{sceneName}' is not available. Generate it from the Tools/Cod Evadare menu and make sure it is in Build Settings.",
            this);
    }

    private static bool IsSceneInBuildSettings(string sceneName, string scenePath)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string buildScenePath = SceneUtility.GetScenePathByBuildIndex(i);

            if (!string.IsNullOrWhiteSpace(scenePath) && buildScenePath == scenePath)
            {
                return true;
            }

            if (Path.GetFileNameWithoutExtension(buildScenePath) == sceneName)
            {
                return true;
            }
        }

        return false;
    }

    public void ShowControls()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(false);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    public void ShowMainMenu()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
        }

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit requested", this);
#else
        Application.Quit();
#endif
    }
}
