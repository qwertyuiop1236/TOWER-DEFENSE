using UnityEngine;

public abstract class Enemy : MonoBehaviour, IPoolable
{
    [Header("Data (назначается в префабе или фабрикой)")]
    [SerializeField] protected EnemyDataSO _data;  // теперь данные из ScriptableObject

    // Характеристики, используемые в коде (кэшированные значения из _data)
    protected float _moveSpeed;
    protected float _health;
    protected float _maxHealth;
    protected float _armor;
    protected float _maxArmor;
    protected int _cost;
    protected int _scoreValue;
    protected int _damageToBase;

    protected int _damage;  // урон, наносимый врагом (может быть переопределён в наследниках)

    private Transform[] waypoints;
    private int currentIndex = 0;

    public static event System.Action<Enemy> OnDeath;

    public int Cost => _cost;
    public float Health => _health;
    public float Armor => _armor;

    // Значения для сброса состояния
    private float _initialSpeed;
    private float _initialHealth;
    private float _initialArmor;
    private int _initialScore;
    private int _initialCost;

    protected virtual void Awake()
    {
        InitializeFromData();
    }

    /// <summary>
    /// Инициализирует врага данными из ScriptableObject. Вызывается при создании (Awake) и при доставании из пула (OnGetFromPool).
    /// </summary>
    public virtual void InitializeFromData()
    {
        if (_data == null)
        {
            Debug.LogError($"Enemy {name} не имеет назначенного EnemyDataSO!", this);
            return;
        }

        _moveSpeed = _data.moveSpeed;
        _health = _data.maxHealth;
        _maxHealth = _data.maxHealth;
        _armor = _data.armor;
        _cost = _data.cost;
        _scoreValue = _data.scoreValue;
        _damageToBase = _data.damageToBase;

        // Сохраняем начальные значения для сброса
        _initialSpeed = _moveSpeed;
        _initialHealth = _health;
        _initialArmor = _armor;
        _initialScore = _scoreValue;
        _initialCost = _cost;

        // Дополнительная инициализация (например, для летающих врагов)
        if (_data.isFlying)
        {
            // можно настроить физику, игнорирование наземных препятствий и т.д.
        }
    }

    protected virtual void Start()
    {
        if (waypoints != null && waypoints.Length > 1)
        {
            waypoints = PathManager.Instance.waypoints;
            if (waypoints == null || waypoints.Length == 0)
                Debug.LogError("Нет точек пути! Добавьте Waypoints в PathManager.");
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

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
                Attack(_damageToBase);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        if (_health - damage > 0)
        {
            _health -= damage;
            PlayDamageSound();
        }
        else
        {
            Death();
        }
    }

    protected virtual void Death()
    {
        OnDeath?.Invoke(this);
        StatsSystem.Instance.AddMoney(_cost);
        StatsSystem.Instance.AddScore(_scoreValue);
        PlayDeathSound();
        if (_data.deathEffect != null)
            Instantiate(_data.deathEffect, transform.position, Quaternion.identity);
        ObjectPool.Instance.Return(gameObject);
    }

    protected virtual void Attack(int damage)
    {
        Debug.Log($"{name} достиг конца пути и нанёс {damage} урона базе!");
        StatsSystem.Instance.TakeDamage(damage);
        ObjectPool.Instance.Return(gameObject);
    }

    protected virtual void PlayDamageSound()
    {
        if (!string.IsNullOrEmpty(_data.damageSoundKey))
            AudioManager.Instance.PlaySound(_data.damageSoundKey, randomPitch: true, position: transform.position);
    }

    protected virtual void PlayDeathSound()
    {
        if (!string.IsNullOrEmpty(_data.deathSoundKey))
            AudioManager.Instance.PlaySound(_data.deathSoundKey, randomPitch: true, position: transform.position);
    }

    public virtual void ResetState()
    {
        _moveSpeed = _initialSpeed;
        _health = _initialHealth;
        _armor = _initialArmor;
        _scoreValue = _initialScore;
        _cost = _initialCost;
        _damage = 0;
        currentIndex = 0;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.Rebind();
    }

    public void OnGetFromPool()
    {
        // При доставании из пула переинициализируем данные (на случай, если данные изменились)
        InitializeFromData();
    }

    public void SetData(EnemyDataSO data)
{
    _data = data;
    InitializeFromData();
}

    public void OnReturnToPool() => ResetState();

    protected virtual void OnEnable() => EnemyRegistry.Register(this);
    protected virtual void OnDisable() => EnemyRegistry.Unregister(this);
}