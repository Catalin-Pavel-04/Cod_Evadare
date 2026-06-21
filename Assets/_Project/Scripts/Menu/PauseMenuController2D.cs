using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController2D : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;

    private void Start()
    {
        SetPaused(false);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(pauseKey))
        {
            return;
        }

        if (!isPaused && IsBlockingUiActive())
        {
            return;
        }

        SetPaused(!isPaused);
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();

        if (activeScene.buildIndex >= 0)
        {
            SceneManager.LoadScene(activeScene.buildIndex);
        }
        else
        {
            SceneManager.LoadScene(activeScene.name);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quit requested", this);
#else
        Application.Quit();
#endif
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        Time.timeScale = isPaused ? 0f : 1f;
    }

    private bool IsBlockingUiActive()
    {
        return IsNamedPanelActive("GameOverPanel")
            || IsNamedPanelActive("VictoryPanel")
            || IsNamedPanelActive("BuffChoicePanel");
    }

    private bool IsNamedPanelActive(string panelName)
    {
        GameObject panel = GameObject.Find(panelName);
        return panel != null && panel.activeInHierarchy;
    }

    private void OnDestroy()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }
}
