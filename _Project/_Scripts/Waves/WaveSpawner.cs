using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    public IEnumerator SpawnWave(WaveData wave)
    {
        foreach (EnemyWave enemyWave in wave.enemies)
        {
            for (int i = 0; i < enemyWave.count; i++)
            {
                EnemyFactory.Create(enemyWave.enemyData, spawnPoint.position, spawnPoint.rotation);
                yield return new WaitForSeconds(enemyWave.delayBetweenSpawn);
            }
        }
    }
}