using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController2D : MonoBehaviour
{
    [SerializeField] private string demoSceneName = "Prototype_FinalDemo";
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        Time.timeScale = 1f;
        ShowMainMenu();
    }

    public void PlayDemo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(demoSceneName);
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
