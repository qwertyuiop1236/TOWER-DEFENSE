using UnityEngine;
using UnityEngine.Events;

public class BuildPad : MonoBehaviour
{
    //[Header("Состояние")]
    public bool IsOccupied { get; private set; }
    public Tower CurrentTower { get; private set; }
    
    [Header("Визуальные элементы")]
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _upgradeUI;
    
    [Header("События")]
    public UnityEvent<Tower> OnTowerBuilt;
    public UnityEvent<Tower> OnTowerSelected;
    
void Start()
{
    // Проверяем наличие 2D коллайдера
    Collider2D collider2D = GetComponent<Collider2D>();
    if (collider2D == null)
    {
        Debug.LogError("BuildPad нужен Collider2D для 2D игры!");
    }
    
    if (_highlight != null) _highlight.SetActive(false);
    if (_upgradeUI != null) _upgradeUI.SetActive(false);
}
    
    // Вызывается при наведении
    public void SetHighlight(bool active)
    {
        if (_highlight != null) _highlight.SetActive(active);
    }
    
    // Вызывается при клике на пад
    public void OnPadClicked()
    {
        Debug.Log($"Кликнут BuildPad! Occupied: {IsOccupied}, TowerData: ");
        
        if (IsOccupied && CurrentTower != null)
        {
            // ... существующий код
        }
        else if (!IsOccupied)
        {
            Debug.Log("BuildPad свободен! Можно строить.");
        }
    }
    
    // Строительство башни
    public bool BuildTower(TowerData towerData)
    {
        if (IsOccupied || towerData == null) return false;
        
        // Создаем башню
        GameObject towerGO = Instantiate(towerData.prefab, transform.position, Quaternion.identity);
        CurrentTower = towerGO.GetComponent<Tower>();
        
        if (CurrentTower == null)
        {
            Debug.LogError($"Префаб {towerData.prefab.name} не содержит Tower!");
            Destroy(towerGO);
            return false;
        }
        
        // Инициализируем с данными
        CurrentTower.Initialize(towerData, this);
        IsOccupied = true;
        
        // Событие
        OnTowerBuilt?.Invoke(CurrentTower);
        
        Debug.Log($"Построена {towerData.towerName}");
        return true;
    }
    
    // Улучшение башни (две ветки как в Kingdom Rush)
    public bool UpgradeTower(int upgradePath)
    {
        if (!IsOccupied || CurrentTower == null) return false;
        
        // Получаем данные улучшения
        TowerUpgrade upgrade = CurrentTower.GetUpgradeData(upgradePath);
        
        if (upgrade == null)
        {
            Debug.Log("Нет доступных улучшений!");
            return false;
        }
        
        // Проверяем деньги
        if (!StatsSystem.Instance.TrySpendMoney(upgrade.cost))
        {
            Debug.Log("Недостаточно денег для улучшения!");
            return false;
        }
        
        // Применяем улучшение
        bool success = CurrentTower.ApplyUpgrade(upgrade, upgradePath);
        
        if (success)
        {
            Debug.Log($"Башня улучшена по ветке {upgradePath}");
            ShowUpgradeUI(false); // Скрываем UI после улучшения
        }
        
        return success;
    }
    
    // Продажа башни
    public int SellTower()
    {
        if (!IsOccupied || CurrentTower == null) return 0;
        
        // Получаем стоимость продажи
        int sellValue = CurrentTower.GetSellPrice();
        
        // Возвращаем деньги
        StatsSystem.Instance.AddMoney(sellValue);
        
        // Уничтожаем башню
        Destroy(CurrentTower.gameObject);
        CurrentTower = null;
        IsOccupied = false;
        
        // Скрываем UI
        ShowUpgradeUI(false);
        
        Debug.Log($"Башня продана за {sellValue}");
        return sellValue;
    }
    
    // Показать/скрыть UI улучшений
    void ShowUpgradeUI(bool show)
    {
        if (_upgradeUI != null)
        {
            _upgradeUI.SetActive(show);
            
            if (show)
            {
                // Обновляем информацию в UI
                UpdateUpgradeUI();
            }
        }
    }
    
    void UpdateUpgradeUI()
    {
        // Здесь обновляем цены, описания и т.д.
        // Можно использовать UIManager для этого
    }
}