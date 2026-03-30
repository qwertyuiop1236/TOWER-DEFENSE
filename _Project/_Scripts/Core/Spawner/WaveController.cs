using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Настройки волн")]
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float initialDelay = 2f; // задержка перед первой волной

    [Header("Ссылки")]
    [SerializeField] private WaveSpawner _waveSpawner;
    [SerializeField] private WaveTimer _waveTimer;

    private int _currentWaveIndex;
    private int _enemiesRemaining;      // количество врагов в текущей волне, которые ещё не убиты

    public static WaveController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        if (_waveSpawner == null)
            _waveSpawner = FindObjectOfType<WaveSpawner>();
        if (_waveTimer == null)
            _waveTimer = FindObjectOfType<WaveTimer>();

        // Подписка на события
        Enemy.OnDeath += OnEnemyDeath;
        GameManager.Instance.OnStateChanged += OnGameStateChanged;
        if (_waveTimer != null)
            _waveTimer.OnTimeEnd += OnTimerEnd;

        // Начинаем с предволнового состояния
        GameManager.Instance.SetState(GameState.PreWave);
        // Запускаем таймер перед первой волной
        _waveTimer.StartTimer(initialDelay);
    }

    void OnDestroy()
    {
        Enemy.OnDeath -= OnEnemyDeath;
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        if (_waveTimer != null)
            _waveTimer.OnTimeEnd -= OnTimerEnd;
    }

    private void OnGameStateChanged(GameState newState)
    {
        // Можно реагировать на смену состояния (например, приостановить спавн)
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        if (GameManager.Instance.CurrentState != GameState.WaveActive)
            return;

        _enemiesRemaining--;
        Debug.Log($"Enemy died, remaining: {_enemiesRemaining}");

        if (_enemiesRemaining <= 0)
        {
            // Волна завершена
            GameManager.Instance.SetState(GameState.WaveComplete);
            // Запускаем таймер до следующей волны
            if (_currentWaveIndex < waves.Length - 1)
            {
                _waveTimer.StartTimer(waves[_currentWaveIndex].delayAfterWave);
            }
            else
            {
                Debug.Log("Все волны пройдены! Победа!");
                GameManager.Instance.SetState(GameState.GameOver);
            }
        }
    }

    private void OnTimerEnd()
    {
        // Таймер закончился – начинаем следующую волну (если есть)
        if (_currentWaveIndex >= waves.Length)
        {
            Debug.Log("Нет больше волн.");
            return;
        }

        StartWave();
    }

    public void StartWave()
    {
        if (_currentWaveIndex >= waves.Length) return;

        WaveData wave = waves[_currentWaveIndex];
        // Подсчитываем общее количество врагов в волне
        int totalEnemies = 0;
        foreach (var enemyWave in wave.enemies)
        {
            totalEnemies += enemyWave.count;
        }
        _enemiesRemaining = totalEnemies;

        GameManager.Instance.SetState(GameState.WaveActive);
        StartCoroutine(_waveSpawner.SpawnWave(wave));
        _currentWaveIndex++;
    }

    // Метод для пропуска волны (кнопка "Skip")
    public void SkipWave()
    {
        if (GameManager.Instance.CurrentState == GameState.PreWave)
        {
            // Прерываем таймер и начинаем волну сразу
            _waveTimer.StopTimer();
            OnTimerEnd();
        }
        else if (GameManager.Instance.CurrentState == GameState.WaveActive)
        {
            // Убиваем всех оставшихся врагов (вызываем их смерть)
            foreach (var enemy in EnemyRegistry.AllEnemies)
            {
                enemy.TakeDamage(9999f);
            }
        }
    }

    // Метод для принудительного начала следующей волны (если волна завершена и таймер не запущен)
    public void ForceNextWave()
    {
        if (GameManager.Instance.CurrentState == GameState.WaveComplete)
        {
            _waveTimer.StartTimer(0); // мгновенный запуск таймера
        }
    }
}