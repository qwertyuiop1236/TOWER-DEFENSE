using System;
using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    public static StatsSystem Instance { get; private set; }
    
    // СОБЫТИЯ
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnEnemiesKilledChanged;
    
    // ДАННЫЕ
    [SerializeField] private int _startMoney = 100;
    [SerializeField] private int _startHealth = 20;
    [SerializeField] private int _startScore = 0;
    
    private int _money;
    private int _score;
    private int _health;
    private int _enemiesKilled;
    
    // СВОЙСТВА (только чтение)
    public int Money => _money;
    public int Score => _score;
    public int Health => _health;
    public int EnemiesKilled => _enemiesKilled;
    
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
    
    void InitializeStats()
    {
        _money = _startMoney;
        _health = _startHealth;
        _score = _startScore;
        _enemiesKilled = 0;
        
        // Уведомляем о начальных значениях
        OnMoneyChanged?.Invoke(_money);
        OnHealthChanged?.Invoke(_health);
        OnScoreChanged?.Invoke(_score);
    }
    
    // МЕТОДЫ ДЛЯ ИЗМЕНЕНИЯ СТАТИСТИКИ

    // Увеличение каличества манет
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        _money += amount;
        OnMoneyChanged?.Invoke(_money);
    }
    
    // Уменьшение каличества манет
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
    
    // Изменение каличества здаровья
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        _health = Mathf.Max(0, _health - damage);
        OnHealthChanged?.Invoke(_health);
    }
    
    // Увеличение каличества здаровью
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _health += amount;
        OnHealthChanged?.Invoke(_health);
    }
    
    // Увеличение количества убийств
    public void EnemyKilled()
    {
        _enemiesKilled++;
        OnEnemiesKilledChanged?.Invoke(_enemiesKilled);
    }
    
    // СБРОС СТАТИСТИКИ (для новой игры)
    public void ResetStats()
    {
        _money = _startMoney;
        _health = _startHealth;
        _score = _startScore;
        _enemiesKilled = 0;
        
        OnMoneyChanged?.Invoke(_money);
        OnHealthChanged?.Invoke(_health);
        OnScoreChanged?.Invoke(_score);
        OnEnemiesKilledChanged?.Invoke(_enemiesKilled);
    }
}
