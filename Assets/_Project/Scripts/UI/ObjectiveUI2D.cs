using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI2D : MonoBehaviour
{
    [SerializeField] private Text objectiveText;
    [SerializeField] private Text hintText;

    public void SetObjective(string objective)
    {
        if (objectiveText != null)
        {
            objectiveText.text = objective ?? string.Empty;
        }
    }

    public void SetHint(string hint)
    {
        if (hintText != null)
        {
            hintText.text = hint ?? string.Empty;
        }
    }

    public void ClearHint()
    {
        if (hintText != null)
        {
            hintText.text = string.Empty;
        }
    }
}
