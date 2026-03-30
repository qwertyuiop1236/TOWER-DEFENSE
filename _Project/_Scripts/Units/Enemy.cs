using UnityEngine;


public abstract class Enemy : MonoBehaviour, IPoolable
{
    // ОБЩИЕ ДЛЯ ВСЕХ ВРАГОВ
    [Header("Общие параметры для всех врагов")]
    [SerializeField] protected float _speedMuve =1f;
    [SerializeField] protected int _cost =100;
    [SerializeField] protected float _maxXp;
    [SerializeField] protected float _xp=100;
    [SerializeField] protected float _maxArmor;
    [SerializeField] protected float _armor=0;
    [SerializeField] protected int _point = 100;


    protected int _damage;
    private Transform[] waypoints; // Массив точек пути
    private int currentIndex = 0;  // Текущая точка

    public int Cost => _cost;
    public float XP => _xp;
    public float Armor => _armor;

    // Переменные для сброса.
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

        if(waypoints != null && waypoints.Length > 1){;
            _maxXp += _xp;
            _maxArmor += _armor;
            // Получаем точки пути от PathManager
            waypoints = PathManager.Instance.waypoints;
            
            // Проверяем что точки есть
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("Нет точек пути! Добавьте Waypoints в PathManager.");
            }
        }
    }
    
    protected virtual void Update()
    {
        // Если точек нет или дошли до конца - выходим
        if (waypoints == null || currentIndex >= waypoints.Length) return;
        
        // Получаем текущую цель
        Transform target = waypoints[currentIndex];
        
        // Двигаемся к цели
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            _speedMuve * Time.deltaTime
        );
        
        // Проверяем достигли ли цели
        float distance = Vector2.Distance(transform.position, target.position);
        if (distance < 0.1f) // Если близко к точке
        {
            // Переходим к следующей точке
            currentIndex++;
            
            // Если это была последняя точка
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
        // Вместо Destroy(gameObject);
        // Сначала добавляем деньги и очки
        StatsSystem.Instance.AddMoney(_cost);
        StatsSystem.Instance.AddScore(_point);
        
        // Возвращаем в пул
        ObjectPool.Instance.Return(gameObject);
    }

    protected virtual void Attack(int Damage)
    {
        Debug.Log("Враг достиг конца пути!");
        StatsSystem.Instance.TakeDamage(Damage);
        // Вместо Destroy(gameObject);
        ObjectPool.Instance.Return(gameObject);
    }

    
    // Метод сброса состояния (вызывается при возврате в пул)
    public virtual void ResetState()
    {
        _speedMuve = _initialSpeed;
        _xp = _initialXp;
        _armor = _initialArmor;
        _point = _initialPoint;
        _damage = 0; // если нужно
        currentIndex = 0; // сброс индекса пути
        // Сброс скорости Rigidbody, если есть
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
        // Сброс аниматора (опционально)
        Animator anim = GetComponent<Animator>();
        if (anim != null) anim.Rebind();
    }

    // Реализация IPoolable
    public void OnGetFromPool()
    {
        // Можно ничего не делать, или выполнить код при доставании
    }

    public void OnReturnToPool()
    {
        ResetState(); // сбрасываем всё перед возвратом
    }
}