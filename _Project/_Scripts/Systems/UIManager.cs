using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Текстовые элементы")]
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _timeBeforeWaveText;  // теперь будет отображать таймер WaveTimer
    
    [Header("Форматирование")]
    [SerializeField] private string _moneyFormat = "$ {0}";
    [SerializeField] private string _scoreFormat = "Score: {0}";
    [SerializeField] private string _healthFormat = "HP: {0}";
    
    void Start()
    {
        // Подписка на события StatsSystem
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnMoneyChanged += UpdateMoneyUI;
            StatsSystem.Instance.OnScoreChanged += UpdateScoreUI;
            StatsSystem.Instance.OnHealthChanged += UpdateHealthUI;
        }
        
        // Подписка на события WaveTimer
        if (WaveTimer.Instance != null)
        {
            WaveTimer.Instance.OnTimeChanged += UpdateTimeUI;
            // Инициализируем отображение таймера текущим значением
            UpdateTimeUI(WaveTimer.Instance.CurrentTime);
        }
        else
        {
            Debug.LogWarning("WaveTimer.Instance не найден! Таймер не будет отображаться.");
        }
        
        // Обновляем начальные значения ресурсов
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
    
    // Метод для обновления отображения таймера (вызывается из WaveTimer)
    public void UpdateTimeUI(float seconds)
    {
        // Если время <= 0, можно показывать "0:00" или "Wave!"
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
        // Отписка от StatsSystem
        if (StatsSystem.Instance != null)
        {
            StatsSystem.Instance.OnMoneyChanged -= UpdateMoneyUI;
            StatsSystem.Instance.OnScoreChanged -= UpdateScoreUI;
            StatsSystem.Instance.OnHealthChanged -= UpdateHealthUI;
        }
        
        // Отписка от WaveTimer
        if (WaveTimer.Instance != null)
        {
            WaveTimer.Instance.OnTimeChanged -= UpdateTimeUI;
        }
    }
}