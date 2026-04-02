using System.Collections;
using UnityEngine;

/// <summary>
/// Управляет волнами врагов: подписывается на события смерти врагов и таймера,
/// отслеживает оставшихся врагов, переключает состояние игры.
/// </summary>
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

    // События для LevelController
    public event System.Action OnAllWavesComplete;
    public event System.Action OnBaseDestroyed;

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

        // Подписка на разрушение базы
        if (StatsSystem.Instance != null)
            StatsSystem.Instance.OnHealthDepleted += OnBaseHealthDepleted;

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
        if (StatsSystem.Instance != null)
            StatsSystem.Instance.OnHealthDepleted -= OnBaseHealthDepleted;
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

        // Если все враги волны убиты и больше нет волн
        if (_enemiesRemaining <= 0 && _currentWaveIndex >= waves.Length)
        {
            // Волна завершена
            GameManager.Instance.SetState(GameState.WaveComplete);
            OnAllWavesComplete?.Invoke();
        }
    }

    private void OnBaseHealthDepleted(int health)
    {
        if (health <= 0)
        {
            OnBaseDestroyed?.Invoke();
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

    /// <summary>Пропуск волны (кнопка "Skip")</summary>
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
            // Убиваем всех оставшихся врагов
            foreach (var enemy in EnemyRegistry.AllEnemies)
            {
                if (enemy != null)
                    enemy.TakeDamage(9999f);
            }
        }
    }

    /// <summary>Принудительно начать следующую волну (если волна завершена и таймер не запущен)</summary>
    public void ForceNextWave()
    {
        if (GameManager.Instance.CurrentState == GameState.WaveComplete)
        {
            _waveTimer.StartTimer(0); // мгновенный запуск таймера
        }
    }
}