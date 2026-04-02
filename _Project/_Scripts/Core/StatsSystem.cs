using System;
using UnityEngine;

public class StatsSystem : MonoBehaviour
{
    public static StatsSystem Instance { get; private set; }
    
    public event Action<int> OnMoneyChanged;
    public event Action<int> OnScoreChanged;
    public event Action<int> OnHealthChanged;
    public event Action<int> OnHealthDepleted;  // добавлено
    
    [SerializeField] private int _startMoney = 100;
    [SerializeField] private int _startHealth = 20;
    [SerializeField] private int _startScore = 0;
    
    private int _money;
    private int _score;
    private int _health;
    
    public int Money => _money;
    public int Score => _score;
    public int Health => _health;
    
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

    // Единый метод TakeDamage
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        _health = Mathf.Max(0, _health - damage);
        OnHealthChanged?.Invoke(_health);
        if (_health == 0) OnHealthDepleted?.Invoke(_health);
    }

    void InitializeStats()
    {
        _money = _startMoney;
        _health = _startHealth;
        _score = _startScore;
        OnMoneyChanged?.Invoke(_money);
        OnHealthChanged?.Invoke(_health);
        OnScoreChanged?.Invoke(_score);
    }
    
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        _money += amount;
        OnMoneyChanged?.Invoke(_money);
    }
    
    public bool TrySpendMoney(int amount)
    {
        if (_money < amount) return false;
        _money -= amount;
        OnMoneyChanged?.Invoke(_money);
        return true;
    }
    
    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        _score += amount;
        OnScoreChanged?.Invoke(_score);
    }
    
    public void Heal(int amount)
    {
        if (amount <= 0) return;
        _health += amount;
        OnHealthChanged?.Invoke(_health);
    }
    
    public void ResetStats()
    {
        _money = _startMoney;
        _health = _startHealth;
        _score = _startScore;
        OnMoneyChanged?.Invoke(_money);
        OnHealthChanged?.Invoke(_health);
        OnScoreChanged?.Invoke(_score);
    }
}