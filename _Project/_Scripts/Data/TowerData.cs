using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TargetStrategyType
{
    Nearest,
    Strongest,
    // AirPriority // можно добавить позже
}


[CreateAssetMenu(fileName = "NewTower", menuName = "Kingdom Rush/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Основные данные")]
    public string towerName;
    public string description;
    public int baseCost;
    public GameObject prefab;
    public GameObject ghostPrefab;

    [Header("Снаряд")]
    public GameObject projectilePrefab;
    
    [Header("Боевые характеристики")]
    public float baseDamage;
    public float baseRange;
    public float baseAttackSpeed;
    public DamageType damageType; // Enum: Physical, Magic, True
    
    [Header("Ветки улучшений (как в Kingdom Rush)")]
    public UpgradeBranch branchA;
    public UpgradeBranch branchB;

    [Header("UI настройки")]
    public Sprite icon;

    [Header("Стратегия поиска цели")]
    public TargetStrategyType targetStrategy = TargetStrategyType.Nearest;
    
    [System.Serializable]
    public class UpgradeBranch
    {
        public string branchName;
        public TowerUpgrade[] upgrades; // Уровни 1, 2, 3
    }
    
    [Header("Звуки (ключи для AudioManager)")]
    public string towerBuildSoundKey = "tower_build";
    public string arrowShootSounKey = "arrow_shoot";
    public string towerUpgradeSoundKey = "tower_upgrade";
}

[System.Serializable]
public class TowerUpgrade
{
    public int cost;
    public float damageBonus;
    public float rangeBonus;
    public float speedBonus;
    public string upgradeName;
    public string description;
    public GameObject visualModel; // Новая модель для этого уровня
}

public enum DamageType { Physical, Magic, True }