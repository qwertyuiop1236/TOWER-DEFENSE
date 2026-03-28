using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonTower : Tower
{
    // Уникальные поля пушки
    [SerializeField] private GameObject _cannonballPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _explosionRadius = 1.5f;
    [SerializeField] private float _projectileSpeed = 8f;
    [SerializeField] private float _splashDamageMultiplier = 0.5f; // 50% урона по площади
    
    protected override void Start()
    {
        base.Start();
        
        // Настройки пушки
        _range = 3f; // Средняя дистанция
        _attackSpeed = 0.4f; // Медленная, но мощная
        _cost = 200;
        _damage = 30f * _level; // Очень высокий урон
        
        Debug.Log("Пушечная башня построена!");
    }
    
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        // Создаем ядро
        GameObject cannonball = Instantiate(_cannonballPrefab, _firePoint.position, Quaternion.identity);
        
        // Рассчитываем траекторию (дуга)
        Vector3 direction = (_currentTarget.transform.position - _firePoint.position).normalized;
        
        // Добавляем движение с "дугой"
        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * _projectileSpeed;
            // Добавляем немного силы вверх для дуги
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }
        
        
        ResetAttackTimer();
        Debug.Log($"Пушка стреляет! Основной урон: {_damage}, по площади: {_damage * _splashDamageMultiplier}");
    }
    
    protected override void FindTarget()
    {
        // Пушка выбирает самого здорового врага (танка)
        Enemy strongestEnemy = null;
        float maxHealth = 0f;
        
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        
        foreach (Enemy enemy in allEnemies)
        {
            if (!IsInRange(enemy.transform.position)) continue;
            
            // Предполагаем что у Enemy есть свойство Health
            // float enemyHealth = enemy.GetHealth();
            // if (enemyHealth > maxHealth)
            // {
            //     maxHealth = enemyHealth;
            //     strongestEnemy = enemy;
            // }
            
            // Временная логика: берем первого врага
            if (strongestEnemy == null)
            {
                strongestEnemy = enemy;
            }
        }
        
        _currentTarget = strongestEnemy;
    }
    
    public override bool Upgrade()
    {
        bool success = base.Upgrade();
        
        if (success)
        {
            // Уникальные улучшения пушки
            _explosionRadius *= 1.3f; // +30% радиуса взрыва
            _splashDamageMultiplier += 0.1f; // +10% урона по площади
            
            Debug.Log($"Пушка улучшена! Радиус взрыва: {_explosionRadius}");
        }
        
        return success;
    }
    
    // Уникальный метод только для пушки
    public void ManualAim(Vector3 position)
    {
        // Ручное наведение (особая способность)
        Debug.Log($"Пушка наведена на {position}");
        // Здесь можно сделать прицеливание
    }
}