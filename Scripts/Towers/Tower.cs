using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    // 1. ОБЩИЕ ДЛЯ ВСЕХ БАШЕН
    [SerializeField] protected float _range = 3;
    [SerializeField] protected float _attackSpeed = 1f; // атак в секунду
    [SerializeField] protected int _cost = 100;
    [SerializeField] protected int _upgradeCost = 50;
    [SerializeField] protected int _level = 1;
    
    // 2. Protected поля
    protected float _damage;
    protected float _attackTimer;
    protected Enemy _currentTarget;
    protected bool _canAttack = true;
    
    // 3. Свойства
    public bool CanAttack => _canAttack && _attackTimer <= 0f;
    
    // 4. Виртуальный Start
    protected virtual void Start()
    {
        _damage = 10f * _level; // Базовый урон
        Debug.Log($"Башня уровня {_level} построена!");
    }
    
    // 5. Виртуальный Update
    protected virtual void Update()
    {
        if (!_canAttack) return;
        
        // Обновляем таймер
            _attackTimer -= Time.deltaTime;

        // Ищем цель если нет текущей
        if (_currentTarget == null || !IsTargetValid(_currentTarget))
        {
            FindTarget();
        }
        
        // Атакуем если есть цель и готовы
        if (CanAttack && _currentTarget != null)
        {
            Attack();
        }
    }
    
    // 6. Абстрактные методы (РАЗНЫЕ для каждой башни)
    public abstract void Attack();
    protected abstract void FindTarget();
    
    // 7. Виртуальные методы (можно переопределить)
    public virtual bool Upgrade()
    {
        if (_level >= 3) return false; // Макс уровень 3
        
        _level++;
        _damage *= 1.5f; // +50% урона за уровень
        _range *= 1.2f; // +20% дальности
        _attackSpeed *= 1.1f; // +10% скорости
        
        Debug.Log($"Башня улучшена до уровня {_level}!");
        return true;
    }
    
    // 8. Общие методы для всех башен
    protected bool IsTargetValid(Enemy enemy)
    {
        if (enemy == null) return false;
       // if (!enemy.IsAlive) return false;
        return IsInRange(enemy.transform.position);
    }
    
    protected bool IsInRange(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= _range;
    }
    
    protected void ResetAttackTimer()
    {
        _attackTimer = 1f / _attackSpeed;
    }
    
    // 9. Отключение/включение башни
    public virtual void Disable()
    {
        _canAttack = false;
        _currentTarget = null;
    }
    
    public virtual void Enable()
    {
        _canAttack = true;
    }
}
