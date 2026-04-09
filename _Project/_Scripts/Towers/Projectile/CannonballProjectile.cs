using UnityEngine;

/// <summary>
/// Снаряд пушки. При попадании создаёт взрыв в области.
/// </summary>
public class CannonballProjectile : ProjectileBase, IPoolable
{
    [Header("Настройки взрыва")]
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private float _splashDamageMultiplier = 0.5f;
    [SerializeField] private GameObject _explosionEffectPrefab;

    private Rigidbody2D _rb;
    private float _damage;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnGetFromPool() { }

    public void OnReturnToPool()
    {
        if (_rb != null) _rb.velocity = Vector2.zero;
    }

    public override void Initialize(float damage, float pierceChance, GameObject owner)
    {
        base.Initialize(damage, pierceChance, owner);
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Игнорируем владельца и другие снаряды
        if (other.gameObject == _owner || other.GetComponent<ProjectileBase>() != null)
            return;

        // Прямое попадание
        Enemy directHit = other.GetComponent<Enemy>();
        if (directHit != null)
        {
            directHit.TakeDamage(_damage);
        }

        // Взрыв по площади
        Explode();

        // Возвращаем снаряд в пул
        ObjectPool.Instance.Return(gameObject);
    }

    private void Explode()
    {
        // Визуальный эффект взрыва
        if (_explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Звук взрыва
        AudioManager.Instance.PlaySound("cannon_explosion", randomPitch: true, position: transform.position);

        // Находим всех врагов в радиусе
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float splashDamage = _damage * _splashDamageMultiplier;
                enemy.TakeDamage(splashDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}