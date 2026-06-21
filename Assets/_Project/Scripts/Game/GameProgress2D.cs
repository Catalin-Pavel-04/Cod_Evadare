using UnityEngine;

public static class GameProgress2D
{
    private const string PlayerPrefsPrefix = "CodEvadare_";
    private const string HighestUnlockedLevelKey = PlayerPrefsPrefix + "HighestUnlockedLevel";
    private const int FirstLevelIndex = 1;
    private const int MaxLevelIndex = 5;

    public static int GetHighestUnlockedLevel()
    {
        return Mathf.Clamp(PlayerPrefs.GetInt(HighestUnlockedLevelKey, FirstLevelIndex), FirstLevelIndex, MaxLevelIndex);
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex >= FirstLevelIndex && levelIndex <= GetHighestUnlockedLevel();
    }

    public static void UnlockLevel(int levelIndex)
    {
        int clampedLevelIndex = Mathf.Clamp(levelIndex, FirstLevelIndex, MaxLevelIndex);

        if (clampedLevelIndex <= GetHighestUnlockedLevel())
        {
            return;
        }

        PlayerPrefs.SetInt(HighestUnlockedLevelKey, clampedLevelIndex);
        PlayerPrefs.Save();
        Debug.Log($"Unlocked level {clampedLevelIndex}.");
    }

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(HighestUnlockedLevelKey, FirstLevelIndex);
        PlayerPrefs.Save();
        Debug.Log("Campaign progress reset.");
    }
}
