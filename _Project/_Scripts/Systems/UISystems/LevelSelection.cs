using UnityEngine.SceneManagement;

public static class LevelSelection
{
    private static int selectedLevelIndex = 0;

    public static void LoadLevel(int levelIndex)
    {
        selectedLevelIndex = levelIndex;
        SceneManager.LoadScene("Gameplay");
    }

    public static int GetSelectedLevel()
    {
        return selectedLevelIndex;
    }
}