using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicTower : Tower
{
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private Transform _castPoint;
    [SerializeField] private float _slowEffect = 0.3f; // Замедление на 30%
    [SerializeField] private float _effectDuration = 3f;
    [SerializeField] private bool _canChain = false;
    [SerializeField] private int _maxChainTargets = 3;
    
    protected override void Start()
    {
        base.Start();
        
        // Настройки магической башни
        _range = 2.5f; // Короткая дистанция
        _attackSpeed = 1.5f; // Быстрая
        _cost = 150;
        _damage = 8f * _level; // Низкий урон, но с эффектами
        
        Debug.Log("Магическая башня построена!");
    }
    
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        // Создаем заклинание
        GameObject spell = Instantiate(_spellPrefab, _castPoint.position, Quaternion.identity);
        
        // Наводим на цель
        Vector3 direction = (_currentTarget.transform.position - _castPoint.position).normalized;
        spell.transform.right = direction;
        
        ResetAttackTimer();
        Debug.Log($"Магия! Урон: {_damage}, Замедление: {_slowEffect:P0}");
    }
    
    protected override void FindTarget()
    {
        // Маг выбирает самого быстрого врага
        Enemy fastestEnemy = null;
        // float maxSpeed = 0f;
        
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        
        foreach (Enemy enemy in allEnemies)
        {
            if (!IsInRange(enemy.transform.position)) continue;
            
            // Временная логика: берем ближайшего врага
            if (fastestEnemy == null)
            {
                fastestEnemy = enemy;
            }
            else
            {
                float currentDistance = Vector3.Distance(transform.position, enemy.transform.position);
                float fastestDistance = Vector3.Distance(transform.position, fastestEnemy.transform.position);
                
                if (currentDistance < fastestDistance)
                {
                    fastestEnemy = enemy;
                }
            }
        }
        
        _currentTarget = fastestEnemy;
    }
    
    public override bool Upgrade()
    {
        bool success = base.Upgrade();
        
        if (success)
        {
            // Уникальные улучшения мага
            _slowEffect += 0.1f; // +10% замедления
            _effectDuration += 0.5f; // +0.5с длительности
            
            if (_level >= 2) _canChain = true;
            if (_level >= 3) _maxChainTargets = 5;
            
            Debug.Log($"Маг улучшен! Цепная молния: {_canChain}, целей: {_maxChainTargets}");
        }
        
        return success;
    }
    
    // Уникальный метод мага
    public void CastAreaSpell(Vector3 position, float radius)
    {
        // Особое заклинание по площади
        Debug.Log($"Заклинание по площади в {position}, радиус: {radius}");
        // Логика АОЕ заклинания
    }
}
