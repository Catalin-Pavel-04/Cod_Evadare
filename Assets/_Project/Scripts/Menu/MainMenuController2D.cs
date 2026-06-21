using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class MainMenuController2D : MonoBehaviour
{
    [SerializeField] private string demoSceneName = "Prototype_FinalDemo";
    [SerializeField] private string demoScenePath = "Assets/_Project/Scenes/Prototype_FinalDemo.unity";
    [SerializeField] private string prisonSceneName = "Prototype_PrisonLevel";
    [SerializeField] private string prisonScenePath = "Assets/_Project/Scenes/Prototype_PrisonLevel.unity";
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

    public void PlayPrisonLevel()
    {
        LoadScene(prisonSceneName, prisonScenePath);
    }

    public void LoadSceneByName(string sceneName)
    {
        LoadScene(sceneName, string.Empty);
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
            $"Scene '{sceneName}' is not available. Run Tools/Cod Evadare/Create Prototype 2.0 Prison Level, then Tools/Cod Evadare/Update Main Menu With Level 2.",
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
