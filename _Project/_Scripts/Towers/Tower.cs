using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : MonoBehaviour
{
    // 1. ОБЩИЕ ДЛЯ ВСЕХ БАШЕН
    [SerializeField] protected float _range = 3;
    [SerializeField] protected float _attackSpeed = 1f;
    [SerializeField] protected int _cost = 100;
    [SerializeField] protected int _upgradeCost = 50;
    [SerializeField] protected int _level = 1;

    [Header("Визуальные компоненты")]
    [SerializeField] private GameObject _currentVisualModel;
    [SerializeField] private Transform _visualAnchor;
    
    // 2. Protected поля
    protected float _damage;
    protected float _attackTimer;
    protected Enemy _currentTarget;
    protected bool _canAttack = true;
    
    // Поля для строительной системы
    [SerializeField] protected BuildPad _myBuildPad;
    [SerializeField] protected TowerData _towerData;
    
    // 3. Свойства
    public bool CanAttack => _canAttack && _attackTimer <= 0f;
    public int Cost => _cost;
    public int Level => _level;
    public float TowerRange => _range;
    public BuildPad MyBuildPad => _myBuildPad;
    public TowerData TowerData => _towerData;

    // Переменные с ключами звука для Башен
    protected string _towerBuildSoundKey;
    protected string _arrowShootSoundKey;
    protected string _towerUpgradeSoundKey;

    protected ITargetStrategy _targetStrategy;

    protected List<Enemy> enemiesInRange = new List<Enemy>();
    private float _rangeCacheTimer;
    private const float RANGE_CACHE_INTERVAL = 0.2f; // обновлять раз в 0.2 секунды    
    protected GameObject _projectilePrefab;
    
    // 4. Виртуальный Start
    protected virtual void Start()
    {
        // Инициализация стратегии
        if (_towerData != null)
        {
            switch (_towerData.targetStrategy)
            {
                case TargetStrategyType.Nearest:
                    _targetStrategy = new NearestTargetStrategy();
                    break;
                case TargetStrategyType.Strongest:
                    _targetStrategy = new StrongestTargetStrategy();
                    break;
                default:
                    _targetStrategy = new NearestTargetStrategy();
                    break;
            }
        }
        else
        {
            _targetStrategy = new NearestTargetStrategy(); // по умолчанию
        }

        // Если есть TowerData - используем его значения
        if (_towerData != null)
        {
            _damage = _towerData.baseDamage;
            _range = _towerData.baseRange;
            _attackSpeed = _towerData.baseAttackSpeed;
        }
        else
        {
            _damage = 10f * _level; // Базовый урон по умолчанию
        }
        
        if (_towerData != null && _towerData.projectilePrefab != null)
        {
            _projectilePrefab = _towerData.projectilePrefab;
        }
        else
        {
            Debug.LogWarning($"У башни {name} нет projectilePrefab в TowerData!", this);
        }
        
        Debug.Log($"Башня уровня {_level} построена! Урон: {_damage}, Дальность: {_range}");
    }
    
    // 5. Виртуальный Update
    protected virtual void Update()
    {
        if (!_canAttack) return;

        // Обновляем таймер атаки
        if (_attackTimer > 0)
            _attackTimer -= Time.deltaTime;

        // Обновляем кэш врагов в радиусе с интервалом
        if (_rangeCacheTimer <= 0f)
        {
            UpdateEnemiesInRange();
            _rangeCacheTimer = RANGE_CACHE_INTERVAL;
        }
        else
        {
            _rangeCacheTimer -= Time.deltaTime;
        }

        // Если нет цели или цель невалидна - ищем новую
        if (_currentTarget == null || !IsTargetValid(_currentTarget))
        {
            FindTarget();
        }

        // Атакуем, если есть цель и таймер готов
        if (CanAttack && _currentTarget != null)
        {
            Attack();
        }
    }
    
    // 6. Абстрактные методы (РАЗНЫЕ для каждой башни)
    public abstract void Attack();
    

    /// <summary>
    /// Ищет цель, используя текущую стратегию (ближайший, самый сильный и т.д.).
    /// </summary>
    protected virtual void FindTarget()
    {
        if (_targetStrategy != null)
            _currentTarget = _targetStrategy.GetTarget(enemiesInRange, transform.position);
        else
            _currentTarget = null;
    }
    
    // 7. Виртуальные методы улучшения (комбинируем старый и новый подход)
    public virtual bool Upgrade()
    {
        if (_level >= 3) 
        {
            Debug.Log("Достигнут максимальный уровень!");
            return false;
        }
        
        // Если есть TowerData, используем систему улучшений из него
        if (_towerData != null)
        {
            // Получаем доступные улучшения
            TowerUpgrade upgradeA = GetUpgradeData(0);
            TowerUpgrade upgradeB = GetUpgradeData(1);
            
            if (upgradeA == null && upgradeB == null)
            {
                Debug.Log("Нет доступных улучшений!");
                return false;
            }
            
            // По умолчанию улучшаем по первой ветке
            return ApplyUpgrade(upgradeA, 0);
        }
        else
        {
            // Старая система улучшений для обратной совместимости
            _level++;
            _damage *= 1.5f;
            _range *= 1.2f;
            _attackSpeed *= 1.1f;
            
            Debug.Log($"Башня улучшена до уровня {_level}!");
            return true;
        }
    }
    
    // 8. Общие методы для всех башен
   protected bool IsTargetValid(Enemy enemy)
    {
        if (enemy == null) return false;
        if (!enemy.gameObject.activeInHierarchy) return false;
        if (!enemy.enabled) return false;
        return IsInRange(enemy.transform.position);
    }
    
    protected bool IsInRange(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= _range;
    }
    
    protected void ResetAttackTimer()
    {
        _attackTimer = 1f / _attackSpeed;
    }
    
    // 9. Отключение/включение башни
    public virtual void Disable()
    {
        _canAttack = false;
        _currentTarget = null;
    }
    
    public virtual void Enable()
    {
        _canAttack = true;
    }
    
    // 10. Метод инициализации для строительной системы
    public virtual void Initialize(TowerData data, BuildPad buildPad)
    {
        _towerData = data;
        _myBuildPad = buildPad;
        
        // Применяем настройки из TowerData
        if (data != null)
        {
            _damage = data.baseDamage;
            _range = data.baseRange;
            _attackSpeed = data.baseAttackSpeed;
            _cost = data.baseCost;


            _towerBuildSoundKey = data.towerBuildSoundKey;
            _arrowShootSoundKey = data.arrowShootSounKey;
            _towerUpgradeSoundKey = data.towerUpgradeSoundKey;


            // ПРОПУСТИ эту проверку - поля нет в TowerData
            // Визуальная модель уже есть в префабе башни
        }
        
        Debug.Log($"Башня {data.towerName} уровня {_level} построена!");
    }
    
    // 11. Метод для продажи зданий
    public virtual int GetSellPrice()
    {
        // Используем TowerData если есть
        if (_towerData != null)
        {
            return GetSellPriceFromData();
        }
        
        // Или старую формулу
        int totalInvested = _cost + (_upgradeCost * (_level - 1));
        return Mathf.RoundToInt(totalInvested * 0.7f);
    }
    
    // Продажа на основе TowerData
    public int GetSellPriceFromData()
    {
        if (_towerData == null) 
        {
            Debug.LogWarning("TowerData не назначен!");
            return GetSellPrice(); // Возвращаем по старой формуле
        }
        
        int baseValue = _towerData.baseCost;
        int upgradeValue = 0;
        
        // Расчет стоимости улучшений
        if (_level > 1)
        {
            // Суммируем стоимость всех примененных улучшений
            TowerUpgrade[] appliedUpgrades = GetAppliedUpgrades();
            foreach (var upgrade in appliedUpgrades)
            {
                if (upgrade != null)
                    upgradeValue += upgrade.cost;
            }
        }
        
        // Возвращаем 70% от общей вложенной суммы
        return Mathf.RoundToInt((baseValue + upgradeValue) * 0.7f);
    }
    
    // Получить все примененные улучшения
    public TowerUpgrade[] GetAppliedUpgrades()
    {
        if (_towerData == null || _level <= 1)
            return new TowerUpgrade[0];
            
        TowerUpgrade[] upgrades = new TowerUpgrade[_level - 1];
        
        // Здесь нужно знать, какие именно улучшения были применены
        // По умолчанию считаем, что все улучшения из первой ветки
        for (int i = 0; i < _level - 1; i++)
        {
            if (i < _towerData.branchA.upgrades.Length)
                upgrades[i] = _towerData.branchA.upgrades[i];
        }
        
        return upgrades;
    }
    
    // 12. Система улучшений с ветками (Kingdom Rush стиль)
    public virtual bool ApplyUpgrade(TowerUpgrade upgrade, int branchIndex)
    {
        if (_level >= 3) 
        {
            Debug.Log("Максимальный уровень достигнут!");
            return false;
        }
        
        if (upgrade == null)
        {
            Debug.LogWarning("Улучшение не найдено!");
            return false;
        }
        
        // Применяем улучшения характеристик
        _level++;
        _damage += upgrade.damageBonus;
        _range += upgrade.rangeBonus;
        _attackSpeed += upgrade.speedBonus;
        
        Debug.Log($"Башня улучшена до уровня {_level}! " +
                 $"Урон: {_damage}, Дальность: {_range}, " +
                 $"Скорость атаки: {_attackSpeed}");
        
        // Обновляем визуальную модель
        if (upgrade.visualModel != null)
        {
            UpdateVisualModel(upgrade.visualModel);
        }
        
        // Визуальные эффекты
        PlayUpgradeEffects();
        
        return true;
    }
    
    // Получить данные улучшения для указанной ветки
    public TowerUpgrade GetUpgradeData(int branchIndex)
    {
        return GetUpgradeData(branchIndex, _level);
    }
    
    // Получить данные улучшения для указанной ветки и текущего уровня
    public TowerUpgrade GetUpgradeData(int branchIndex, int currentLevel)
    {
        if (_towerData == null) 
        {
            Debug.LogWarning("TowerData не назначен!");
            return null;
        }
        
        // Проверяем, доступно ли улучшение для текущего уровня
        if (currentLevel > 3) // Макс уровень
            return null;
            
        int upgradeIndex = currentLevel - 1; // Уровень 1 -> улучшение 0
        
        if (branchIndex == 0 && upgradeIndex < _towerData.branchA.upgrades.Length)
            return _towerData.branchA.upgrades[upgradeIndex];
        
        if (branchIndex == 1 && upgradeIndex < _towerData.branchB.upgrades.Length)
            return _towerData.branchB.upgrades[upgradeIndex];
        
        return null;
    }
    
    // 13. Обновление визуальной модели
    protected virtual void UpdateVisualModel(GameObject newModelPrefab)
    {
        if (newModelPrefab == null)
        {
            Debug.LogWarning("Нет новой модели для обновления!");
            return;
        }
        
        // Удаляем старую модель, если есть
        if (_currentVisualModel != null)
        {
            Destroy(_currentVisualModel);
        }
        
        // Создаем новую модель
        Transform parent = GetVisualAnchor();
        _currentVisualModel = Instantiate(newModelPrefab, parent);
        _currentVisualModel.transform.localPosition = Vector3.zero;
        _currentVisualModel.transform.localRotation = Quaternion.identity;
        _currentVisualModel.transform.localScale = Vector3.one;
        
        Debug.Log($"Визуальная модель обновлена: {newModelPrefab.name}");
    }
    
    // Получить якорь для визуальной модели
    private Transform GetVisualAnchor()
    {
        if (_visualAnchor != null)
            return _visualAnchor;
        
        // Ищем существующий якорь
        Transform existingAnchor = transform.Find("VisualAnchor");
        if (existingAnchor != null)
            return existingAnchor;
        
        // Создаем новый якорь
        GameObject anchorObj = new GameObject("VisualAnchor");
        anchorObj.transform.SetParent(transform);
        anchorObj.transform.localPosition = Vector3.zero;
        anchorObj.transform.localRotation = Quaternion.identity;
        _visualAnchor = anchorObj.transform;
        
        return _visualAnchor;
    }
    
    // 14. Визуальные эффекты улучшения
    protected virtual void PlayUpgradeEffects()
    {
        // Простой эффект - изменение цвета на мгновение
        StartCoroutine(FlashUpgradeEffect());
    }
    
    private System.Collections.IEnumerator FlashUpgradeEffect()
    {
        // Сохраняем оригинальный цвет
        Color originalColor = GetComponent<Renderer>().material.color;
        
        // Меняем на золотой
        GetComponent<Renderer>().material.color = Color.yellow;
        
        // Ждем 0.3 секунды
        yield return new WaitForSeconds(0.3f);
        
        // Возвращаем оригинальный цвет
        GetComponent<Renderer>().material.color = originalColor;
    }
    
    // 15. Вспомогательные методы для UI
    public string GetUpgradeDescription(int branchIndex)
    {
        TowerUpgrade upgrade = GetUpgradeData(branchIndex);
        if (upgrade == null)
            return "МАКСИМАЛЬНЫЙ УРОВЕНЬ";
            
        return $"{upgrade.upgradeName}\n" +
               $"Цена: {upgrade.cost}G\n" +
               $"Урон: +{upgrade.damageBonus}\n" +
               $"Дальность: +{upgrade.rangeBonus}";
    }
    
    public bool CanUpgrade(int branchIndex)
    {
        if (_level >= 3) return false;
        
        TowerUpgrade upgrade = GetUpgradeData(branchIndex);
        if (upgrade == null) return false;
        
        // Проверяем достаточно ли денег
        return StatsSystem.Instance.Money >= upgrade.cost;
    }
    
    // 16. Отображение радиуса атаки (для дебага)
    void OnDrawGizmosSelected()
    {
        // Рисуем радиус атаки в редакторе
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _range);
        
        // Рисуем линию к текущей цели
        if (_currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
        }
        
    }

    protected void UpdateEnemiesInRange()
    {
        enemiesInRange.Clear();
        var allEnemies = EnemyRegistry.AllEnemies;
        for (int i = 0; i < allEnemies.Count; i++)
        {
            var enemy = allEnemies[i];
            // Проверяем: враг существует, активен, включён и находится в радиусе
            if (enemy != null && enemy.gameObject.activeInHierarchy && enemy.enabled && IsInRange(enemy.transform.position))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }
}