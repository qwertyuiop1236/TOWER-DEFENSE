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
        
        // Проигрывание звука стройки

        AudioManager.Instance.PlaySound("tower_build");

        Debug.Log("Магическая башня построена!");
    }
    
    public override void Attack()
    {
        if (_currentTarget == null) return;
        
        // Было: GameObject spell = Instantiate(_spellPrefab, _castPoint.position, Quaternion.identity);
        GameObject spell = ObjectPool.Instance.Get(_spellPrefab, _castPoint.position, Quaternion.identity);
        
        Vector3 direction = (_currentTarget.transform.position - _castPoint.position).normalized;
        spell.transform.right = direction;
        
        Rigidbody2D rb = spell.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direction * _attackSpeed;
        
        AudioManager.Instance.PlaySound("magic_shoot", randomPitch: true);

        spell.GetComponent<ProjectileBase>().Initialize(_damage, 10, gameObject);
        ResetAttackTimer();
        Debug.Log($"Магия! Урон: {_damage}, Замедление: {_slowEffect:P0}");
    }

    
    protected override void FindTarget()
    {
        Enemy closest = null;
        float minDist = float.MaxValue;
        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        _currentTarget = closest;
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
            
            // Проигрывание звука улучшения
            AudioManager.Instance.PlaySound("tower_upgrade");

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
