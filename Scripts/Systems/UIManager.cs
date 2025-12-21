using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Текстовые элементы")]
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _timeText;
    
    [Header("Форматирование")]
    [SerializeField] private string _moneyFormat = "$ {0}";
    [SerializeField] private string _scoreFormat = "Score: {0}";
    [SerializeField] private string _healthFormat = "HP: {0}";
    [SerializeField] private string _killsFormat = "Kills: {0}";
    
    void Start()
    {
        // ПОДПИСКА на события
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnMoneyChanged += UpdateMoneyUI;
            StatsSystem.Instance.OnScoreChanged += UpdateScoreUI;
            StatsSystem.Instance.OnHealthChanged += UpdateHealthUI;
            StatsSystem.Instance.OnEnemiesKilledChanged += UpdateKillsUI;
        }
        
        // Обновляем начальные значения
        UpdateAllUI();
    }
    
    void UpdateAllUI()
    {
        if (StatsSystem.Instance == null) return;
        
        _moneyText.text = string.Format(_moneyFormat, StatsSystem.Instance.Money);
        _scoreText.text = string.Format(_scoreFormat, StatsSystem.Instance.Score);
        _healthText.text = string.Format(_healthFormat, StatsSystem.Instance.Health);
        _killsText.text = string.Format(_killsFormat, StatsSystem.Instance.EnemiesKilled);
    }
    
    void UpdateMoneyUI(int money) => _moneyText.text = string.Format(_moneyFormat, money);
    void UpdateScoreUI(int score) => _scoreText.text = string.Format(_scoreFormat, score);
    void UpdateHealthUI(int health) => _healthText.text = string.Format(_healthFormat, health);
    void UpdateKillsUI(int kills) => _killsText.text = string.Format(_killsFormat, kills);
    
    // Для таймера (если нужен)
    public void UpdateTimeUI(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        _timeText.text = $"{minutes:00}:{secs:00}";
    }
    
    // ОТПИСКА при уничтожении (важно для избежания утечек памяти)
    void OnDestroy()
    {
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
            StatsSystem.Instance.OnScoreChanged -= UpdateScoreUI;
            StatsSystem.Instance.OnHealthChanged -= UpdateHealthUI;
            StatsSystem.Instance.OnEnemiesKilledChanged -= UpdateKillsUI;
        }
    }
}