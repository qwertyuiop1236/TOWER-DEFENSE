using UnityEngine;

/// <summary>
/// Базовый класс для всех врагов. Управляет движением по пути, получением урона,
/// регистрацией в реестре и возвратом в пул.
/// </summary>
public abstract class Enemy : MonoBehaviour, IPoolable
{
    [Header("Общие параметры")]
    [SerializeField] protected float _moveSpeed = 1f;
    [SerializeField] protected int _cost = 100;
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _health = 100;
    [SerializeField] protected float _maxArmor;
    [SerializeField] protected float _armor = 0;
    [SerializeField] protected int _scoreValue = 100;

    protected int _damage;
    private Transform[] waypoints;
    private int currentIndex = 0;

    public static event System.Action<Enemy> OnDeath;

    public int Cost => _cost;
    public float Health => _health;
    public float Armor => _armor;

    private float _initialSpeed;
    private float _initialHealth;
    private float _initialArmor;
    private int _initialScore;

    protected virtual void Start()
    {
        _initialSpeed = _moveSpeed;
        _initialHealth = _health;
        _initialArmor = _armor;
        _initialScore = _scoreValue;

        if (waypoints != null && waypoints.Length > 1)
        {
            _maxHealth += _health;
            _maxArmor += _armor;
            waypoints = PathManager.Instance.waypoints;
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("Нет точек пути! Добавьте Waypoints в PathManager.");
            }
        }
    }

    protected virtual void Update()
    {
        if (waypoints == null || currentIndex >= waypoints.Length) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            _moveSpeed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                Attack(_damage);
            }
        }
    }

    public virtual void TakeDamage(float Damage)
    {
        if (_health - Damage > 0)
        {
            _health -= Damage;
        }
        else
        {
            Death();
        }
    }

    /// <summary>
    /// Вызывается при смерти врага. Оповещает подписчиков, начисляет награду и возвращает объект в пул.
    /// </summary>
    protected virtual void Death()
    {
        OnDeath?.Invoke(this);
        StatsSystem.Instance.AddMoney(_cost);
        StatsSystem.Instance.AddScore(_scoreValue);
        ObjectPool.Instance.Return(gameObject);
    }

    protected virtual void Attack(int Damage)
    {
        Debug.Log("Враг достиг конца пути!");
        StatsSystem.Instance.TakeDamage(Damage);
        ObjectPool.Instance.Return(gameObject);
    }

    /// <summary>
    /// Сбрасывает состояние врага перед возвратом в пул.
    /// </summary>
    public virtual void ResetState()
    {
        _moveSpeed = _initialSpeed;
        _health = _initialHealth;
        _armor = _initialArmor;
        _scoreValue = _initialScore;
        _damage = 0;
        currentIndex = 0;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.Rebind();
    }

    public void OnGetFromPool() { }
    public void OnReturnToPool() => ResetState();

    protected virtual void OnEnable() => EnemyRegistry.Register(this);
    protected virtual void OnDisable() => EnemyRegistry.Unregister(this);
}