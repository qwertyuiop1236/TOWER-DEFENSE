using UnityEngine;


public abstract class Enemy : MonoBehaviour
{
    // ОБЩИЕ ДЛЯ ВСЕХ ВРАГОВ
    [Header("Общие параметры для всех врагов")]
    [SerializeField] protected float _speedMuve =1f;
    [SerializeField] protected int _cost =100;
    [SerializeField] protected float _xp=100;
    [SerializeField] protected Canvas canvasText;
    [SerializeField] protected int _point = 100;


    protected int _damage;
    private Transform[] waypoints; // Массив точек пути
    private int currentIndex = 0;  // Текущая точка

    public int Cost => _cost;
    public float XP =>_xp;
    
    protected virtual void Start()
    {
        // Получаем точки пути от PathManager
        waypoints = PathManager.Instance.waypoints;
        
        // Проверяем что точки есть
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Нет точек пути! Добавьте Waypoints в PathManager.");
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
        // Удуление врага 
        Destroy(gameObject);

        // Враг НЕ знает про UI, только про экономику
        StatsSystem.Instance.AddMoney(_cost);
        // Повышает количество очков
        StatsSystem.Instance.AddScore(_point);
        // Увеличение каличества убийств



        // StatsSystem.Instance.EnemyKilled();
        // OnDeath?.Invoke(this);

        
    }

    protected virtual void Attack(int Damage)
    {
        // TODO: Нанести урон игроку
        Debug.Log("Враг достиг конца пути!");

        // Нанесение урона врагом
        StatsSystem.Instance.TakeDamage(Damage);

        
        // Уничтожаем врага
        Destroy(gameObject);
    }
}