using UnityEngine;

public abstract class Enemy : MonoBehaviour, IPoolable
{
    // ОБЩИЕ ДЛЯ ВСЕХ ВРАГОВ
    [Header("Общие параметры для всех врагов")]
    [SerializeField] protected float _speedMuve = 1f;
    [SerializeField] protected int _cost = 100;
    [SerializeField] protected float _maxXp;
    [SerializeField] protected float _xp = 100;
    [SerializeField] protected float _maxArmor;
    [SerializeField] protected float _armor = 0;
    [SerializeField] protected int _point = 100;

    protected int _damage;
    private Transform[] waypoints; // Массив точек пути
    private int currentIndex = 0;  // Текущая точка

    // СТАТИЧЕСКОЕ СОБЫТИЕ (для подписки из WaveController)
    public static event System.Action<Enemy> OnDeath;

    // Свойства
    public int Cost => _cost;
    public float Health => _xp;    // для стратегии Strongest
    public float Armor => _armor;

    // Переменные для сброса состояния (пул)
    private float _initialSpeed;
    private float _initialXp;
    private float _initialArmor;
    private int _initialPoint;

    protected virtual void Start()
    {
        _initialSpeed = _speedMuve;
        _initialXp = _xp;
        _initialArmor = _armor;
        _initialPoint = _point;

        if (waypoints != null && waypoints.Length > 1)
        {
            _maxXp += _xp;
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
            _speedMuve * Time.deltaTime
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
        if (_xp - Damage > 0)
        {
            _xp -= Damage;
        }
        else
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        // Вызываем СТАТИЧЕСКОЕ событие (передаём this)
        OnDeath?.Invoke(this);

        StatsSystem.Instance.AddMoney(_cost);
        StatsSystem.Instance.AddScore(_point);
        ObjectPool.Instance.Return(gameObject);
    }

    protected virtual void Attack(int Damage)
    {
        Debug.Log("Враг достиг конца пути!");
        StatsSystem.Instance.TakeDamage(Damage);
        ObjectPool.Instance.Return(gameObject);
    }

    // Сброс состояния (IPoolable)
    public virtual void ResetState()
    {
        _speedMuve = _initialSpeed;
        _xp = _initialXp;
        _armor = _initialArmor;
        _point = _initialPoint;
        _damage = 0;
        currentIndex = 0;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.Rebind();
    }

    // Реализация IPoolable
    public void OnGetFromPool() { }
    public void OnReturnToPool() => ResetState();

    // Регистрация в реестре врагов
    protected virtual void OnEnable()
    {
        EnemyRegistry.Register(this);
    }

    protected virtual void OnDisable()
    {
        EnemyRegistry.Unregister(this);
    }
}