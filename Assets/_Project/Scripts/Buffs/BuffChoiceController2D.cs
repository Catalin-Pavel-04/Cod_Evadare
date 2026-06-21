using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffChoiceController2D : MonoBehaviour
{
    [SerializeField] private PlayerBuffs2D playerBuffs;
    [SerializeField] private BuffDefinition2D[] buffPool;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private Button[] choiceButtons;
    [SerializeField] private Text[] choiceTexts;
    [SerializeField] private Image[] choiceIcons;
    [SerializeField] private Image[] choiceCardFrames;
    [SerializeField] private Text[] choiceNameTexts;
    [SerializeField] private Text[] choiceDescriptionTexts;
    [SerializeField] private bool pauseGameWhileChoosing = true;
    [SerializeField] private int choicesToShow = 3;

    private bool isShowing;
    private bool pausedByThisController;

    public void ShowChoices()
    {
        if (isShowing)
        {
            return;
        }

        if (playerBuffs == null)
        {
            playerBuffs = FindObjectOfType<PlayerBuffs2D>();
        }

        if (playerBuffs == null)
        {
            Debug.LogWarning("Buff choices cannot be shown because PlayerBuffs2D is missing.", this);
            return;
        }

        if (choiceButtons == null || choiceButtons.Length == 0)
        {
            Debug.LogWarning("Buff choices cannot be shown because no choice buttons are assigned.", this);
            return;
        }

        List<BuffDefinition2D> selectedBuffs = PickRandomBuffs();

        if (selectedBuffs.Count == 0)
        {
            Debug.LogWarning("Buff choices cannot be shown because the buff pool is empty.", this);
            return;
        }

        isShowing = true;

        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }

        ConfigureButtons(selectedBuffs);

        if (pauseGameWhileChoosing)
        {
            pausedByThisController = true;
            Time.timeScale = 0f;
        }
    }

    private List<BuffDefinition2D> PickRandomBuffs()
    {
        List<BuffDefinition2D> availableBuffs = new List<BuffDefinition2D>();

        if (buffPool != null)
        {
            foreach (BuffDefinition2D buff in buffPool)
            {
                if (buff != null && !availableBuffs.Contains(buff))
                {
                    availableBuffs.Add(buff);
                }
            }
        }

        int count = Mathf.Min(Mathf.Max(1, choicesToShow), availableBuffs.Count);
        List<BuffDefinition2D> selectedBuffs = new List<BuffDefinition2D>(count);

        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, availableBuffs.Count);
            selectedBuffs.Add(availableBuffs[index]);
            availableBuffs.RemoveAt(index);
        }

        return selectedBuffs;
    }

    private void ConfigureButtons(List<BuffDefinition2D> selectedBuffs)
    {
        int buttonCount = choiceButtons != null ? choiceButtons.Length : 0;

        for (int i = 0; i < buttonCount; i++)
        {
            Button button = choiceButtons[i];

            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveAllListeners();
            bool hasBuff = i < selectedBuffs.Count;
            button.gameObject.SetActive(hasBuff);

            if (!hasBuff)
            {
                continue;
            }

            BuffDefinition2D buff = selectedBuffs[i];

            if (choiceTexts != null && i < choiceTexts.Length && choiceTexts[i] != null)
            {
                choiceTexts[i].text = $"{buff.DisplayName}\n{buff.Description}";
            }

            if (choiceNameTexts != null && i < choiceNameTexts.Length && choiceNameTexts[i] != null)
            {
                choiceNameTexts[i].text = buff.DisplayName;
            }

            if (choiceDescriptionTexts != null && i < choiceDescriptionTexts.Length && choiceDescriptionTexts[i] != null)
            {
                choiceDescriptionTexts[i].text = buff.Description;
            }

            if (choiceIcons != null && i < choiceIcons.Length && choiceIcons[i] != null)
            {
                choiceIcons[i].sprite = buff.Icon;
                choiceIcons[i].enabled = buff.Icon != null;
            }

            if (choiceCardFrames != null && i < choiceCardFrames.Length && choiceCardFrames[i] != null)
            {
                choiceCardFrames[i].enabled = true;
            }

            button.onClick.AddListener(() => ChooseBuff(buff));
        }
    }

    private void ChooseBuff(BuffDefinition2D buff)
    {
        if (!isShowing)
        {
            return;
        }

        playerBuffs.ApplyBuff(buff);
        PlayBuffFeedback();
        HideChoices();
    }

    private void HideChoices()
    {
        ClearButtonListeners();

        if (choicePanel != null)
        {
            choicePanel.SetActive(false);
        }

        isShowing = false;

        if (pausedByThisController)
        {
            pausedByThisController = false;
            Time.timeScale = 1f;
        }
    }

    private void ClearButtonListeners()
    {
        if (choiceButtons == null)
        {
            return;
        }

        foreach (Button button in choiceButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }

    private void OnDestroy()
    {
        ClearButtonListeners();

        if (pausedByThisController)
        {
            Time.timeScale = 1f;
        }
    }

    private void PlayBuffFeedback()
    {
        DemoAudioManager2D audioManager = FindObjectOfType<DemoAudioManager2D>();

        if (audioManager != null)
        {
            audioManager.PlayBuff();
        }
    }

    private void OnValidate()
    {
        choicesToShow = Mathf.Max(1, choicesToShow);
    }
}
