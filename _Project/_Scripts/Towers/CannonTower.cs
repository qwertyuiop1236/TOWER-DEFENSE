using UnityEngine;

public class CannonTower : Tower
{
    [Header("Уникальные поля пушки")]
    [SerializeField] private GameObject _cannonballPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bombSpeed;
    [SerializeField] private float _explosionRadius = 1.5f;
    [SerializeField] private float _projectileSpeed = 8f;
    [SerializeField] private float _splashDamageMultiplier = 0.5f;

    protected override void Start()
    {
        base.Start();
        AudioManager.Instance.PlaySound(_towerBuildSoundKey, volume: 0.5f);
        Debug.Log("Пушечная башня построена!");
    }

    public override void Attack()
    {
        if (_currentTarget == null) return;

        // Создаём снаряд через фабрику
        GameObject cannonball = ProjectileFactory.Create(_cannonballPrefab, _firePoint.position, Quaternion.identity);

        // Направление к цели
        Vector3 direction = (_currentTarget.transform.position - _firePoint.position).normalized;
        Rigidbody2D rb = cannonball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * _bombSpeed;
            // Добавляем небольшую дугу для реализма
            rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
        }


        // Передаём параметры снаряду
        CannonballProjectile projectile = cannonball.GetComponent<CannonballProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(_damage, 0f, gameObject);
        }

        AudioManager.Instance.PlaySound(_arrowShootSoundKey, randomPitch: true);
        ResetAttackTimer();
        Debug.Log($"Пушка стреляет! Основной урон: {_damage}, по площади: {_damage * _splashDamageMultiplier}");
    }

    public override bool Upgrade()
    {
        bool success = base.Upgrade();
        if (success)
        {
            _explosionRadius *= 1.3f;
            _splashDamageMultiplier += 0.1f;
            AudioManager.Instance.PlaySound(_towerUpgradeSoundKey);
            Debug.Log($"Пушка улучшена! Радиус взрыва: {_explosionRadius}");
        }
        return success;
    }
}