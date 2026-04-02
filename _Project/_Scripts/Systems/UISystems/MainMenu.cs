using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel; // ссылка на панель настроек (префаб)

    public void OnNewGame()
    {
        ProgressManager.ResetProgress();
        // Загружаем первый уровень (индекс 0)
        LevelSelection.LoadLevel(0);
    }

    public void OnLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void OnSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OnExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}