using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowTower : Tower
{
    // 1. УНИКАЛЬНЫЕ поля для арбалета
    [Header("Уникальные поля для арбалета")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed = 15f;
    [SerializeField] private float _pierceChance = 0.2f; // 20% шанс пробить
    [SerializeField] private float _damage = 150f;

    
    // 2. Переопределяем Start для настройки
    protected override void Start()
    {
        base.Start(); // Важно: вызываем родительский!
        
        // Настройки арбалета
        _range = _range*1; // Дальняя дистанцияz

        _damage = _damage * _level; // Высокий урон
        
        Debug.Log("Арбалетная башня построена!");
    }
    
    // 3. Реализуем ОБЯЗАТЕЛЬНЫЙ метод Attack
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        // Создаем стрелу
        GameObject arrow = Instantiate(_arrowPrefab, _firePoint.position, Quaternion.identity);
        
        // Направляем на врага
        Vector3 direction = (_currentTarget.transform.position - _firePoint.position).normalized;
        arrow.transform.right = direction;
        
        // Добавляем движение
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * _arrowSpeed;
        }
        
        // Настройка стрелы

        
        // Сброс таймера
        ResetAttackTimer();
        
        Debug.Log($"Арбалет стреляет! Урон: {_damage}");
    }
    
    // 4. Реализуем ОБЯЗАТЕЛЬНЫЙ метод FindTarget
    protected override void FindTarget()
    {
        // Ищем самого дальнего врага в радиусе (стратегия арбалета)
        Enemy farthestEnemy = null;
        float farthestDistance = 0f;
        
        // Получаем всех врагов
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();
        
        foreach (Enemy enemy in allEnemies)
        {
            if (!IsInRange(enemy.transform.position)) continue;
            
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestEnemy = enemy;
            }
        }
        
        _currentTarget = farthestEnemy;
    }
    
    // 5. Переопределяем Upgrade для уникальных улучшений
    public override bool Upgrade()
    {
        bool success = base.Upgrade(); // Вызываем родительский апгрейд
        
        if (success)
        {
            // Уникальные улучшения арбалета
            _pierceChance += 0.15f; // +15% шанс пробития за уровень
            _arrowSpeed *= 1.2f; // +20% скорости стрелы
            
            Debug.Log($"Арбалет улучшен! Шанс пробития: {_pierceChance:P0}");
        }
        
        return success;
    }
}