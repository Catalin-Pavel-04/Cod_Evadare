using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndController2D : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Text victoryText;
    [SerializeField] private string restartKey = "r";
    [SerializeField] private bool pauseOnVictory = true;
    [SerializeField] private int completedLevelIndex;
    [SerializeField] private int unlockLevelIndex;
    [SerializeField] private string nextLevelSceneName;
    [SerializeField] private bool showNextLevelButton = true;
    [SerializeField] private GameObject nextLevelButtonObject;

    private bool victoryActive;

    private void Start()
    {
        HideVictory();
    }

    private void Update()
    {
        if (!victoryActive)
        {
            return;
        }

        if (!string.IsNullOrEmpty(restartKey) && Input.GetKeyDown(restartKey))
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
    }

    public void ShowVictory(string message = "LEVEL CLEARED")
    {
        victoryActive = true;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryText != null)
        {
            victoryText.text = $"{message}\nPress R to restart";
        }

        UnlockProgress();

        if (nextLevelButtonObject != null)
        {
            nextLevelButtonObject.SetActive(showNextLevelButton && !string.IsNullOrWhiteSpace(nextLevelSceneName));
        }

        if (pauseOnVictory)
        {
            Time.timeScale = 0f;
        }

        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayVictory();
        }

        Debug.Log(message, this);
    }

    public void HideVictory()
    {
        victoryActive = false;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        if (nextLevelButtonObject != null)
        {
            nextLevelButtonObject.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void LoadNextLevel()
    {
        if (string.IsNullOrWhiteSpace(nextLevelSceneName))
        {
            Debug.LogWarning("Cannot load next level because nextLevelSceneName is empty.", this);
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelSceneName);
    }

    private void UnlockProgress()
    {
        if (completedLevelIndex <= 0)
        {
            return;
        }

        int levelToUnlock = unlockLevelIndex > 0 ? unlockLevelIndex : completedLevelIndex + 1;
        GameProgress2D.UnlockLevel(levelToUnlock);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void OnValidate()
    {
        completedLevelIndex = Mathf.Max(0, completedLevelIndex);
        unlockLevelIndex = Mathf.Max(0, unlockLevelIndex);
    }
}
