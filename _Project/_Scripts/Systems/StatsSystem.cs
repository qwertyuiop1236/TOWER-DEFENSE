using System;
using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    public static StatsSystem Instance { get; private set; }
    
    // СОБЫТИЯ
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHealthChanged;
    public event Action<float> OnTimeBeforeWaveChanged; // Исправлено имя события
    
    // ДАННЫЕ
    [SerializeField] private int _startMoney = 100;
    [SerializeField] private int _startHealth = 20;
    [SerializeField] private int _startScore = 0;
    [SerializeField] private float _startTimeBeforeWave = 300f; // Исправлено имя
    
    private int _money;
    private int _score;
    private int _health;
    private float _timeBefore; // Исправлено имя
    
    // СВОЙСТВА (только чтение)
    public int Money => _money;
    public int Score => _score;
    public int Health => _health;
    public float TimeBefore => _timeBefore; // Исправлено имя
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStats();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_timeBefore > 0)
        {
            float previousTime = _timeBefore;
            _timeBefore -= Time.deltaTime;
            
            // Важно: вызываем событие только если время действительно изменилось
            // (больше чем на 0.1 секунды, чтобы не вызывать каждый кадр)
            if (Mathf.FloorToInt(previousTime) != Mathf.FloorToInt(_timeBefore))
            {
                OnTimeBeforeWaveChanged?.Invoke(_timeBefore);
            }
            
            // Проверка окончания таймера
            if (_timeBefore <= 0)
            {
                _timeBefore = 0;
                OnTimeBeforeWaveChanged?.Invoke(0);
                // Здесь можно добавить логику начала волны
                Debug.Log("Время вышло! Начинается волна!");
            }
        }
    }

    void InitializeStats()
    {
        _money = _startMoney;
        _health = _startHealth;
        _score = _startScore;
        _timeBefore = _startTimeBeforeWave;
        
        // Уведомляем о начальных значениях
        OnMoneyChanged?.Invoke(_money);
        OnHealthChanged?.Invoke(_health);
        OnScoreChanged?.Invoke(_score);
        OnTimeBeforeWaveChanged?.Invoke(_timeBefore);
    }
    
    // МЕТОДЫ ДЛЯ ИЗМЕНЕНИЯ СТАТИСТИКИ

    // Увеличение количества монет
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        _money += amount;
        OnMoneyChanged?.Invoke(_money);
    }
    
    // Уменьшение количества монет
    public bool TrySpendMoney(int amount)
    {
        if (_money < amount) return false;
        _money -= amount;
        OnMoneyChanged?.Invoke(_money);
        return true;
    }
    
    // Увеличение счета
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        _score += amount;
        OnScoreChanged?.Invoke(_score);
    }
    
    // Изменение количества здоровья
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        _health = Mathf.Max(0, _health - damage);
        OnHealthChanged?.Invoke(_health);
    }
    
    // Увеличение количества здоровья
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _health += amount;
        OnHealthChanged?.Invoke(_health);
    }
    
    // СБРОС СТАТИСТИКИ (для новой игры)
    public void ResetStats()
    {
        _money = _startMoney;
        _health = _startHealth;
        _score = _startScore;
        _timeBefore = _startTimeBeforeWave;
        
        OnMoneyChanged?.Invoke(_money);
        OnHealthChanged?.Invoke(_health);
        OnScoreChanged?.Invoke(_score);
        OnTimeBeforeWaveChanged?.Invoke(_timeBefore);
    }

    // СБРОС ТАЙМЕРА ОЖИДАНИЯ ВОЛН (для подготовки к следующей волне)
    public void ResetTimeBefore() // Исправлено имя метода
    {
        _timeBefore = _startTimeBeforeWave;
        OnTimeBeforeWaveChanged?.Invoke(_timeBefore);
    }
}
