using System.IO;
using UnityEngine;

public static class ProgressManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "progress.json");
    private static ProgressData data;

    static ProgressManager()
    {
        Load();
    }

    public static void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<ProgressData>(json);
        }
        else
        {
            // Начальные значения
            data = new ProgressData
            {
                lastUnlockedLevel = 1,  // уровень 0 открыт всегда
                levelCompleted = new bool[0] // будет расширяться
            };
            Save();
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
    }

    public static bool IsLevelUnlocked(int levelIndex)
    {
        return levelIndex < data.lastUnlockedLevel;
    }

    public static bool IsLevelCompleted(int levelIndex)
    {
        return levelIndex < data.levelCompleted.Length && data.levelCompleted[levelIndex];
    }

    public static void UnlockLevel(int levelIndex)
    {
        if (levelIndex >= data.lastUnlockedLevel)
        {
            data.lastUnlockedLevel = levelIndex + 1;
            Save();
        }
    }

    public static void MarkLevelCompleted(int levelIndex)
    {
        if (data.levelCompleted.Length <= levelIndex)
        {
            System.Array.Resize(ref data.levelCompleted, levelIndex + 1);
        }
        data.levelCompleted[levelIndex] = true;
        Save();
    }

    public static void ResetProgress()
    {
        data = new ProgressData
        {
            lastUnlockedLevel = 1,
            levelCompleted = new bool[0]
        };
        Save();
    }

    public static int GetTotalLevelsCount() => 10; // или получать из конфига
}