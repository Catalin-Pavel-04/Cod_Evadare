using UnityEngine;

public class LevelFlowController2D : MonoBehaviour
{
    [SerializeField] private ObjectiveUI2D objectiveUI;
    [SerializeField] private LevelEndController2D levelEndController;
    [SerializeField] private string levelName = "Laboratory";
    [SerializeField] private string startingObjective = "Escape the Laboratory";
    [SerializeField] private string controlsHint = "WASD move | Mouse aim | Left click shoot | R reload | E interact";

    private void Start()
    {
        Time.timeScale = 1f;

        if (objectiveUI == null)
        {
            objectiveUI = FindObjectOfType<ObjectiveUI2D>();
        }

        if (objectiveUI != null)
        {
            objectiveUI.SetObjective(startingObjective);
            objectiveUI.SetHint(controlsHint);
        }

        Debug.Log($"{levelName} level flow started.", this);
    }

    public void ShowLevelComplete()
    {
        if (levelEndController != null)
        {
            levelEndController.ShowVictory("LABORATORY CLEARED");
            return;
        }

        Debug.Log($"{levelName} cleared.", this);
    }
}
