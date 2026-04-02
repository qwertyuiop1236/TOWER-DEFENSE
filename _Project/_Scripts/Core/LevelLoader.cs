using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Transform levelContainer;
    [SerializeField] private GameObject[] levelPrefabs;
    private static LevelLoader _instance;
    private GameObject currentLevelInstance;
    private LevelController currentLevelController;
    private int currentLevelIndex;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        currentLevelIndex = LevelSelection.GetSelectedLevel();
        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int index)
    {
        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);

        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError($"Level index {index} out of range!");
            return;
        }

        GameObject prefab = levelPrefabs[index];
        if (prefab == null)
        {
            Debug.LogError($"Level prefab for index {index} not found!");
            return;
        }

        currentLevelInstance = Instantiate(prefab, levelContainer);
        currentLevelController = currentLevelInstance.GetComponent<LevelController>();
        if (currentLevelController != null)
        {
            currentLevelController.Initialize(index);
            currentLevelController.OnLevelComplete += OnLevelComplete;
            currentLevelController.OnLevelFailed += OnLevelFailed;
        }
    }

    private void OnLevelComplete()
    {
        int levelIndex = LevelSelection.GetSelectedLevel();
        ProgressManager.MarkLevelCompleted(levelIndex);
        if (!ProgressManager.IsLevelUnlocked(levelIndex + 1))
            ProgressManager.UnlockLevel(levelIndex + 1);
        UIManager.Instance.ShowVictoryPanel(levelIndex);
    }

    private void OnLevelFailed()
    {
        UIManager.Instance.ShowDefeatPanel();
    }

    // Статический метод для перезагрузки текущего уровня
    public static void ReloadCurrentLevel()
    {
        if (_instance != null)
        {
            _instance.LoadLevel(LevelSelection.GetSelectedLevel());
        }
    }
}