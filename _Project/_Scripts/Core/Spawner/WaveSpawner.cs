using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    // Спавн волны по данным
    public IEnumerator SpawnWave(WaveData wave)
    {
        if (wave == null || wave.enemies == null || wave.enemies.Length == 0)
        {
            Debug.LogWarning("Волна пустая!");
            yield break;
        }

        foreach (EnemyWave enemyWave in wave.enemies)
        {
            if (enemyWave.enemyPrefab == null) continue;

            for (int i = 0; i < enemyWave.count; i++)
            {
                // Используем пул
                ObjectPool.Instance.Get(enemyWave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(enemyWave.delayBetweenSpawn);
            }
        }
    }
}