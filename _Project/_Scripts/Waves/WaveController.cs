using System.Collections;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Настройки волн")]
    [SerializeField] private WaveData[] waves;
    [SerializeField] private float initialDelay = 2f;

    [Header("Ссылки")]
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private WaveTimer waveTimer;

    private int currentWaveIndex = 0;
    private int enemiesRemaining = 0;
    private bool isWaveActive = false;
    private bool isWaitingBetweenWaves = false;

    public event System.Action OnAllWavesComplete;
    public event System.Action OnBaseDestroyed;

    public static WaveController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (waveSpawner == null) waveSpawner = FindObjectOfType<WaveSpawner>();
        if (waveTimer == null) waveTimer = FindObjectOfType<WaveTimer>();

        Enemy.OnDeath += OnEnemyDeath;
        GameManager.Instance.OnStateChanged += OnGameStateChanged;
        waveTimer.OnTimeEnd += OnTimerEnd;
        StatsSystem.Instance.OnHealthDepleted += OnBaseHealthDepleted;

        GameManager.Instance.SetState(GameState.PreWave);
        waveTimer.StartTimer(initialDelay);
    }

    private void OnDestroy()
    {
        Enemy.OnDeath -= OnEnemyDeath;
        GameManager.Instance.OnStateChanged -= OnGameStateChanged;
        waveTimer.OnTimeEnd -= OnTimerEnd;
        StatsSystem.Instance.OnHealthDepleted -= OnBaseHealthDepleted;
    }

    private void OnGameStateChanged(GameState newState) { }

    private void OnEnemyDeath(Enemy enemy)
    {
        if (!isWaveActive) return;
        enemiesRemaining--;
        Debug.Log($"[Wave] Enemy died. Remaining: {enemiesRemaining}");

        if (enemiesRemaining <= 0 && isWaveActive)
        {
            isWaveActive = false;
            GameManager.Instance.SetState(GameState.WaveComplete);
            Debug.Log($"[Wave] Wave {currentWaveIndex} completed.");

            // Есть ли следующая волна?
            if (currentWaveIndex + 1 < waves.Length)
            {
                // Запускаем таймер паузы, используя delayAfterWave текущей волны
                float delay = waves[currentWaveIndex].delayAfterWave;
                isWaitingBetweenWaves = true;
                waveTimer.StartTimer(delay);
                Debug.Log($"[Wave] Waiting {delay} seconds before next wave...");
            }
            else
            {
                Debug.Log("[Wave] All waves completed!");
                OnAllWavesComplete?.Invoke();
            }
        }
    }

    private void OnBaseHealthDepleted(int health)
    {
        if (health <= 0) OnBaseDestroyed?.Invoke();
    }

    private void OnTimerEnd()
    {
        // Если таймер закончился во время ожидания между волнами
        if (isWaitingBetweenWaves)
        {
            isWaitingBetweenWaves = false;
            currentWaveIndex++; // переходим к следующей волне
            StartWave();
        }
        // Если таймер закончился перед первой волной
        else if (!isWaveActive && currentWaveIndex == 0 && !isWaitingBetweenWaves)
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        if (currentWaveIndex >= waves.Length) return;

        WaveData wave = waves[currentWaveIndex];
        // Подсчитываем общее количество врагов в этой волне
        int total = 0;
        foreach (var ew in wave.enemies)
            total += ew.count;
        enemiesRemaining = total;

        isWaveActive = true;
        GameManager.Instance.SetState(GameState.WaveActive);
        StartCoroutine(waveSpawner.SpawnWave(wave));
        Debug.Log($"[Wave] Started wave {currentWaveIndex} with {total} enemies.");
    }

    public void SkipWave()
    {
        if (GameManager.Instance.CurrentState == GameState.PreWave)
        {
            waveTimer.StopTimer();
            OnTimerEnd();
        }
        else if (GameManager.Instance.CurrentState == GameState.WaveActive && isWaveActive)
        {
            foreach (var enemy in EnemyRegistry.AllEnemies)
                enemy?.TakeDamage(9999f);
        }
        else if (isWaitingBetweenWaves)
        {
            waveTimer.StopTimer();
            OnTimerEnd();
        }
    }

    public void ForceNextWave()
    {
        if (GameManager.Instance.CurrentState == GameState.WaveComplete && !isWaveActive && !isWaitingBetweenWaves)
        {
            // Если волна завершена, но таймер не запущен (например, баг), запускаем сразу
            if (currentWaveIndex + 1 < waves.Length)
            {
                currentWaveIndex++;
                StartWave();
            }
        }
    }
}