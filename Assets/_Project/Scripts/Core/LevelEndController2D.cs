using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndController2D : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Text victoryText;
    [SerializeField] private string restartKey = "r";
    [SerializeField] private bool pauseOnVictory = true;

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

        if (pauseOnVictory)
        {
            Time.timeScale = 0f;
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

        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
