using UnityEngine;

public class MagicTower : Tower
{
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private Transform _castPoint;
    [SerializeField] private float _slowEffect = 0.3f; // Замедление на 30%
    [SerializeField] private float _effectDuration = 3f;
    [SerializeField] private bool _canChain = false;
    [SerializeField] private int _maxChainTargets = 3;
    
    protected override void Start()
    {
        base.Start();
        
        Debug.Log("Магическая башня построена!");
    }
    
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        // Создаем заклинание
        GameObject spell = Instantiate(_spellPrefab, _castPoint.position, Quaternion.identity);
        
        // Наводим на цель
        Vector3 direction = (_currentTarget.transform.position - _castPoint.position).normalized;
        spell.transform.right = direction;
        
        // Добавляем движение
        Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * _attackSpeed;
        }
        // Настройка стрелы
        spell.GetComponent<ArrowShells>().Initialize(_damage,10,gameObject);

        ResetAttackTimer();
        
        Debug.Log($"Магия! Урон: {_damage}, Замедление: {_slowEffect:P0}");
    }

private float _searchTimer;
private const float SEARCH_INTERVAL = 0.5f; // Искать раз в полсекунды
    
protected override void FindTarget()
{
    // Ищем самого ближайшего врага в радиусе
    Enemy closestEnemy = null;
    float closestDistance = float.MaxValue; // Начинаем с максимального значения

    _searchTimer += Time.deltaTime;
    if (_searchTimer < SEARCH_INTERVAL) return;
    _searchTimer = 0f;
    
    // Получаем всех врагов
    Enemy[] allEnemies = FindObjectsOfType<Enemy>();
    
    foreach (Enemy enemy in allEnemies)
    {
        if (!IsInRange(enemy.transform.position)) continue;
        
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        
        // Ищем минимальное расстояние (ближайший враг)
        if (distance < closestDistance)
        {
            closestDistance = distance;
            closestEnemy = enemy;
        }
    }
    
    _currentTarget = closestEnemy;
}

    public override bool Upgrade()
    {
        bool success = base.Upgrade();
        
        if (success)
        {
            // Уникальные улучшения мага
            _slowEffect += 0.1f; // +10% замедления
            _effectDuration += 0.5f; // +0.5с длительности
            
            if (_level >= 2) _canChain = true;
            if (_level >= 3) _maxChainTargets = 5;
            
            Debug.Log($"Маг улучшен! Цепная молния: {_canChain}, целей: {_maxChainTargets}");
        }
        
        return success;
    }
    
    // Уникальный метод мага
    public void CastAreaSpell(Vector3 position, float radius)
    {
        // Особое заклинание по площади
        Debug.Log($"Заклинание по площади в {position}, радиус: {radius}");
        // Логика АОЕ заклинания
    }
}
