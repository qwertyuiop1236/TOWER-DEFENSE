using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Текстовые элементы")]
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _timeBeforeWaveText;
    
    [Header("Форматирование")]
    [SerializeField] private string _moneyFormat = "$ {0}";
    [SerializeField] private string _scoreFormat = "Score: {0}";
    [SerializeField] private string _healthFormat = "HP: {0}";

    [Header("Панели")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private Button nextLevelButton;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnMoneyChanged += UpdateMoneyUI;
            StatsSystem.Instance.OnScoreChanged += UpdateScoreUI;
            StatsSystem.Instance.OnHealthChanged += UpdateHealthUI;
        }
        
        if (WaveTimer.Instance != null)
        {
            WaveTimer.Instance.OnTimeChanged += UpdateTimeUI;
            UpdateTimeUI(WaveTimer.Instance.CurrentTime);
        }
        else
        {
            Debug.LogWarning("WaveTimer.Instance не найден! Таймер не будет отображаться.");
        }
        
        UpdateAllUI();
    }
    
    void UpdateAllUI()
    {
        if (StatsSystem.Instance == null) return;
        _moneyText.text = string.Format(_moneyFormat, StatsSystem.Instance.Money);
        _scoreText.text = string.Format(_scoreFormat, StatsSystem.Instance.Score);
        _healthText.text = string.Format(_healthFormat, StatsSystem.Instance.Health);
    }
    
    void UpdateMoneyUI(int money) => _moneyText.text = string.Format(_moneyFormat, money);
    void UpdateScoreUI(int score) => _scoreText.text = string.Format(_scoreFormat, score);
    void UpdateHealthUI(int health) => _healthText.text = string.Format(_healthFormat, health);
    
    public void UpdateTimeUI(float seconds)
    {
        if (seconds <= 0f)
        {
            _timeBeforeWaveText.text = "Wave!";
            return;
        }
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        _timeBeforeWaveText.text = $"{minutes:00}:{secs:00}";
    }
    
    void OnDestroy()
    {
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
            StatsSystem.Instance.OnScoreChanged -= UpdateScoreUI;
            StatsSystem.Instance.OnHealthChanged -= UpdateHealthUI;
        }
        if (WaveTimer.Instance != null)
        {
            WaveTimer.Instance.OnTimeChanged -= UpdateTimeUI;
        }
    }

    public void ShowVictoryPanel(int levelIndex)
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (nextLevelButton != null)
            nextLevelButton.interactable = ProgressManager.IsLevelUnlocked(levelIndex + 1);
    }

    public void ShowDefeatPanel()
    {
        if (defeatPanel != null) defeatPanel.SetActive(true);
    }

    public void OnRestartLevel()
    {
        LevelLoader.ReloadCurrentLevel();
    }

    public void OnNextLevel()
    {
        int next = LevelSelection.GetSelectedLevel() + 1;
        if (ProgressManager.IsLevelUnlocked(next))
            LevelSelection.LoadLevel(next);
    }

    public void OnBackToLevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }
}