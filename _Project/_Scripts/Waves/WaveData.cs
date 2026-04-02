using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Kingdom Rush/Wave Data")]
public class WaveData : ScriptableObject
{
    public EnemyWave[] enemies;
    public float delayAfterWave = 3f;
}

[System.Serializable]
public class EnemyWave
{
    public EnemyDataSO enemyData;   // вместо GameObject enemyPrefab
    public int count = 1;
    public float delayBetweenSpawn = 0.5f;
}