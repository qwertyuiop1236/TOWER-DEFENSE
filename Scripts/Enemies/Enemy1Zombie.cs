using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZombie : Enemy
{
    [Header("настройка уникальных параметров Zombie")]
    [SerializeField] protected float speedMuveZombie;    
    [SerializeField] protected int damageZombie;
    [SerializeField] protected float xpZombie = 200f;

    [SerializeField] protected bool hasShield = false;
    [SerializeField] protected float shieldDamageMultiplier = 1f;
    
    // НОВОЕ: Добавляем поля для управления скоростью
private float originalSpeed;
    private bool isBoosted = false;

    protected override void Start()
    {
        base.Start();
        
        originalSpeed = _speedMuve;
        

        _damage = damageZombie;
        _xp = xpZombie;
    }

    // Методы для управления скоростью
    public void ApplySpeedBoost(float multiplier)
    {
        if (!isBoosted)
        {
            isBoosted = true;
            _speedMuve = originalSpeed * multiplier;
            Debug.Log($"{name} ускорен в {multiplier} раз");
        }
    }

    public void ResetSpeed()
    {
        if (isBoosted)
        {
            isBoosted = false;
            _speedMuve = originalSpeed;
            Debug.Log($"{name} скорость сброшена");
        }
    }

    // Остальной код остается без изменений...
    public override void TakeDamage(float damage)
    {
        if (hasShield)
        {
            damage *= shieldDamageMultiplier;
        }
        base.TakeDamage(damage);
        Debug.Log("нанесенный урон " + damage);
    }

    public void ApplyShield(float damageReduction, float duration)
    {
        hasShield = true;
        shieldDamageMultiplier = damageReduction;
        // Визуальный эффект щита
        // Запустить таймер для снятия щита
    }

    public void RemoveShield()
    {
        hasShield = false;
        shieldDamageMultiplier = 1f;
        // Убрать визуальный эффект
    }
}