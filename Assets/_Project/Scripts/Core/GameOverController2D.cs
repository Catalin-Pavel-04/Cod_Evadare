using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController2D : MonoBehaviour
{
    [SerializeField] private PlayerHealth2D playerHealth;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private string restartKey = "r";

    private bool isGameOver;

    private void Start()
    {
        Time.timeScale = 1f;

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth2D>();
        }

        if (playerHealth != null)
        {
            playerHealth.PlayerDied += HandlePlayerDied;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isGameOver)
        {
            return;
        }

        if (!string.IsNullOrEmpty(restartKey) && Input.GetKeyDown(restartKey))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.PlayerDied -= HandlePlayerDied;
        }

        Time.timeScale = 1f;
    }

    private void HandlePlayerDied()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;
        Debug.Log("Game Over. Press R to restart.", this);
    }
}
