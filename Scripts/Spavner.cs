using System.Collections;
using UnityEngine;

public class SimpleWaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyWave
    {
        public GameObject enemyPrefab;
        public int count = 1;
        public float delayBetweenSpawn = 0.5f;
    }

    [System.Serializable]
    public class Wave
    {
        public EnemyWave[] enemies;
        public float delayAfterWave = 3f;
    }

    public Wave[] waves;
    public Transform spawnPoint;

    private int currentWaveIndex = 0;

    void Start()
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
        
        StartCoroutine(SpawnAllWaves());
    }

    IEnumerator SpawnAllWaves()
    {
        Debug.Log("Начинаем спавн волн");
        
        while (currentWaveIndex < waves.Length)
        {
            Debug.Log($"Начинаем волну {currentWaveIndex + 1}");
            
            // Спавним текущую волну
            yield return StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            
            // Ждем перед следующей волной
            yield return new WaitForSeconds(waves[currentWaveIndex].delayAfterWave);
            
            currentWaveIndex++;
        }
        
        Debug.Log("Все волны завершены");
    }
    
    IEnumerator SpawnWave(Wave wave)
    {
        if (wave.enemies == null || wave.enemies.Length == 0)
        {
            Debug.LogWarning("Волна пустая!");
            yield break;
        }

        foreach (EnemyWave enemyWave in wave.enemies)
        {
            if (enemyWave.enemyPrefab == null)
            {
                Debug.LogError("Не указан префаб врага!");
                continue;
            }

            Debug.Log($"Спавним {enemyWave.count} врагов типа: {enemyWave.enemyPrefab.name}");

            for (int i = 0; i < enemyWave.count; i++)
            {
                // Создаем врага
                Instantiate(enemyWave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                
                // Ждем перед следующим врагом
                if (enemyWave.delayBetweenSpawn > 0)
                {
                    yield return new WaitForSeconds(enemyWave.delayBetweenSpawn);
                }
                else
                {
                    yield return null; // Один кадр
                }
            }
        }
    }

    // Методы для управления из других скриптов
    public void StartWaves()
    {
        StopAllCoroutines();
        currentWaveIndex = 0;
        StartCoroutine(SpawnAllWaves());
    }

    public void StopWaves()
    {
        StopAllCoroutines();
    }

    public void SkipToWave(int waveIndex)
    {
        StopAllCoroutines();
        currentWaveIndex = Mathf.Clamp(waveIndex, 0, waves.Length - 1);
        StartCoroutine(SpawnAllWaves());
    }
}