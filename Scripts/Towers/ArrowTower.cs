using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowTower : Tower
{

    // 1. УНИКАЛЬНЫЕ поля для арбалета
    [Header("Уникальные поля для арбалета")]
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _arrowSpeed;
    [SerializeField] private float _pierceChance = 0.2f; // 20% шанс пробить
    
    // Настройка Общих параметров
    [SerializeField] private float _rangeArrow = 5f;
    [SerializeField] private float _attackSpeedArrow = 1;
    [SerializeField] private int  _costArrow = 15;
    [SerializeField] private int _damageArrow = 50;


    // 2. Переопределяем Start для настройки
    protected override void Start()
    {
        base.Start(); // Важно: вызываем родительский!
        
        // Настройки параметров арбалета
        _range = _rangeArrow; // Дальняя дистанцияz
        _attackSpeed = _attackSpeedArrow; // Скорость стрельбы
        _cost = _costArrow; // Каличество поинтов
        _damage = _damageArrow; // Высокий урон
        
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
        arrow.GetComponent<ArrowShells>().Initialize(_damage,_pierceChance,gameObject);
        
        // Сброс таймера
        ResetAttackTimer();
        
        Debug.Log($"Арбалет стреляет! Урон: {_damage}");
    }
    
    // 4. Реализуем ОБЯЗАТЕЛЬНЫЙ метод FindTarget
    private float _searchTimer;
    private const float SEARCH_INTERVAL = 0.5f; // Искать раз в полсекунды
    protected override void FindTarget()
    {
        // Ищем самого дальнего врага в радиусе (стратегия арбалета)
        Enemy farthestEnemy = null;
        float farthestDistance = 0f;

        _searchTimer += Time.deltaTime;
        if (_searchTimer < SEARCH_INTERVAL) return;
        _searchTimer = 0f;
        
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