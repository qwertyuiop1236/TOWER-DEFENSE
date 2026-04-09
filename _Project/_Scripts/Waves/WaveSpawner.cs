using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    public IEnumerator SpawnWave(WaveData wave)
    {
        // Запускаем спавн каждой группы в отдельной корутине (параллельно)
        foreach (EnemyWave enemyWave in wave.enemies)
        {
            StartCoroutine(SpawnGroup(enemyWave));
        }
        // Ждём, пока все группы закончат спавн (но не обязательно - можно просто выйти, а спавн продолжится в фоне)
        // Однако WaveController должен знать, когда все враги появились? Счётчик врагов пополняется при создании каждого врага.
        // Поэтому просто запускаем и завершаем корутину.
        yield return null; // даём старт корутинам
        // На самом деле нужно дождаться окончания спавна всех групп? Не обязательно,
        // так как враги могут появляться долго, а счётчик _enemiesRemaining уже увеличен при создании.
        // Но чтобы WaveController не завершил волну раньше времени, он следит за _enemiesRemaining.
    }

    private IEnumerator SpawnGroup(EnemyWave group)
    {
        // Ждём startDelay
        if (group.startDelay > 0)
            yield return new WaitForSeconds(group.startDelay);

        for (int i = 0; i < group.count; i++)
        {
            EnemyFactory.Create(group.enemyData, spawnPoint.position, spawnPoint.rotation);
            if (group.delayBetweenSpawn > 0)
                yield return new WaitForSeconds(group.delayBetweenSpawn);
            else
                yield return null; // один кадр
        }
    }
}