using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;

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
                EnemyFactory.Create(enemyWave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(enemyWave.delayBetweenSpawn);
            }
        }
    }
}