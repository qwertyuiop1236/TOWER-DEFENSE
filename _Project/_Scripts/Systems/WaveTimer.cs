using System;
using UnityEngine;

public class WaveTimer : MonoBehaviour
{
    public static WaveTimer Instance { get; private set; }
    
    public event Action<float> OnTimeChanged;   // вызывается при изменении времени
    public event Action OnTimeEnd;              // вызывается, когда время истекло
    
    [SerializeField] private float _startTime = 5f;  // начальное время между волнами (можно менять)
    private float _currentTime;
    private bool _isRunning;
    
    public float CurrentTime => _currentTime;
    public bool IsRunning => _isRunning;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Update()
    {
        if (!_isRunning) return;
        
        _currentTime -= Time.deltaTime;
        OnTimeChanged?.Invoke(_currentTime);
        
        if (_currentTime <= 0f)
        {
            _currentTime = 0f;
            _isRunning = false;
            OnTimeChanged?.Invoke(0f);
            OnTimeEnd?.Invoke();
        }
    }
    
    /// <summary>Запустить таймер с указанным временем (сек).</summary>
    public void StartTimer(float duration)
    {
        _currentTime = duration;
        _isRunning = true;
        OnTimeChanged?.Invoke(_currentTime);
    }
    
    /// <summary>Остановить таймер без вызова OnTimeEnd.</summary>
    public void StopTimer()
    {
        _isRunning = false;
    }
    
    /// <summary>Сбросить таймер к начальному значению (не запуская).</summary>
    public void ResetTimer()
    {
        _currentTime = _startTime;
        OnTimeChanged?.Invoke(_currentTime);
    }
    
    /// <summary>Установить новое стартовое время.</summary>
    public void SetStartTime(float newStartTime)
    {
        _startTime = newStartTime;
        if (!_isRunning) _currentTime = _startTime;
    }
}