using UnityEngine;

/// <summary>
/// Данные врага для настройки в инспекторе.
/// </summary>
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Kingdom Rush/Enemy Data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Основные параметры")]
    public string enemyName;
    public GameObject prefab;

    [Header("Характеристики")]
    public float moveSpeed = 1f;
    public float maxHealth = 100f;
    public float armor = 0f;
    public int cost = 100;          // деньги за убийство
    public int scoreValue = 100;    // очки за убийство
    public int damageToBase = 10;   // урон базе при достижении конца пути

    [Header("Специальные свойства")]
    public bool isFlying = false;

    [Header("Визуальные эффекты")]
    public Sprite icon;
    public GameObject deathEffect;

    [Header("Звуки (ключи для AudioManager)")]
    public string spawnSoundKey = "enemy_spawn";
    public string damageSoundKey = "enemy_damage";
    public string deathSoundKey = "enemy_death";
    public string armorHitSoundKey = "enemy_armor_hit";
}