using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Kingdom Rush/Wave Data")]
public class WaveData : ScriptableObject
{
    public EnemyWave[] enemies;    // массив групп врагов
    public float delayAfterWave = 3f;  // время до следующей волны
}

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int count = 1;
    public float delayBetweenSpawn = 0.5f;
}