using System;
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
    // [SerializeField] private float _rangeArrow = 5f;
    // [SerializeField] private float _attackSpeedArrow = 1;
    // [SerializeField] private int  _costArrow = 15;
    // [SerializeField] private int _damageArrow = 50;


    // 2. Переопределяем Start для настройки
    protected override void Start()
    {
        base.Start(); // Важно: вызываем родительский!
        
        // Проигрывание звука стройки
        AudioManager.Instance.PlaySound("tower_build", volume: 0.5f);


        Debug.Log("Арбалетная башня построена!");
    }
    
    // 3. Реализуем ОБЯЗАТЕЛЬНЫЙ метод Attack
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        // Вместо Instantiate:
        // GameObject arrow = Instantiate(_arrowPrefab, _firePoint.position, Quaternion.identity);
        GameObject arrow = ProjectileFactory.Create(_arrowPrefab, _firePoint.position, Quaternion.identity);
    
        Vector3 direction = (_currentTarget.transform.position - _firePoint.position).normalized;
        arrow.transform.right = direction;
        
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direction * _arrowSpeed;
        
        AudioManager.Instance.PlaySound("arrow_shoot", randomPitch: true);
        arrow.GetComponent<ProjectileBase>().Initialize(_damage, _pierceChance, gameObject);
        ResetAttackTimer();
        Debug.Log($"Арбалет стреляет! Урон: {_damage}");
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

        // Проигрывание звука улучшения
        AudioManager.Instance.PlaySound("tower_upgrade");
        
        return success;
    }
}