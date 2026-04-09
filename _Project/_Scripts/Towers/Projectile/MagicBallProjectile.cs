using UnityEngine;

public class MagicBallProjectille : ProjectileBase, IPoolable
{
    private Rigidbody2D _rb;

    private void Awake() => _rb = GetComponent<Rigidbody2D>();

    public void OnGetFromPool() { }
    public void OnReturnToPool()
    {
        if (_rb != null) _rb.velocity = Vector2.zero;
    }

    public override void Initialize(float damage, float pierceChance, GameObject owner)
    {
        base.Initialize(damage, pierceChance, owner);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _owner || other.GetComponent<ProjectileBase>()) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage((int)_damageShells);
            if (Random.value > _pierceChanceArrow)
                ObjectPool.Instance.Return(gameObject);
        }
        else
        {
            ObjectPool.Instance.Return(gameObject);
        }
    }
}