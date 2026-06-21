using UnityEngine;
using UnityEngine.UI;

public class BuffStatusUI2D : MonoBehaviour
{
    [SerializeField] private PlayerBuffs2D playerBuffs;
    [SerializeField] private Text buffStatusText;

    private void Start()
    {
        if (playerBuffs == null)
        {
            playerBuffs = FindObjectOfType<PlayerBuffs2D>();
        }

        if (playerBuffs != null)
        {
            playerBuffs.BuffApplied += HandleBuffApplied;
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (playerBuffs != null)
        {
            playerBuffs.BuffApplied -= HandleBuffApplied;
        }
    }

    private void HandleBuffApplied(BuffDefinition2D buff)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (buffStatusText == null)
        {
            return;
        }

        if (playerBuffs == null || playerBuffs.AppliedBuffCount == 0)
        {
            buffStatusText.text = "Buffs: none";
            return;
        }

        buffStatusText.text = $"Buffs: {playerBuffs.AppliedBuffCount} | Last: {playerBuffs.LastAppliedBuffName}";
    }
}
