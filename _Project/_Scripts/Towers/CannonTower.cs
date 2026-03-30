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
        
        AudioManager.Instance.PlaySound("tower_build", volume: 0.5f);
        Debug.Log("Пушечная башня построена!");
    }
    
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        GameObject cannonball = ProjectileFactory.Create(_cannonballPrefab, _firePoint.position, Quaternion.identity);
    
        Vector3 direction = (_currentTarget.transform.position - _firePoint.position).normalized;
        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * _projectileSpeed;
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }
        
        AudioManager.Instance.PlaySound("arrow_shoot", randomPitch: true);
        ResetAttackTimer();
        Debug.Log($"Пушка стреляет! Основной урон: {_damage}, по площади: {_damage * _splashDamageMultiplier}");
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