using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerUpgradeUI : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private Button _upgradePathAButton;
    [SerializeField] private Button _upgradePathBButton;
    [SerializeField] private Button _sellButton;
    
    [SerializeField] private TMP_Text _upgradePathAText;
    [SerializeField] private TMP_Text _upgradePathBText;
    [SerializeField] private TMP_Text _sellText;
    
    [Header("Текущая башня")]
    private Tower _selectedTower;
    private BuildPad _selectedPad;
    
    void Start()
    {
        // Скрываем панель при старте
        _upgradePanel.SetActive(false);
        
        // Назначаем обработчики кнопок
        _upgradePathAButton.onClick.AddListener(() => UpgradeTower(0));
        _upgradePathBButton.onClick.AddListener(() => UpgradeTower(1));
        _sellButton.onClick.AddListener(SellTower);
    }
    
    // Показать UI для башни
    public void ShowForTower(Tower tower, BuildPad pad)
    {
        _selectedTower = tower;
        _selectedPad = pad;
        
        if (tower == null || pad == null)
        {
            HideUI();
            return;
        }
        
        // Позиционируем панель над башней
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tower.transform.position);
        _upgradePanel.transform.position = screenPos + new Vector3(0, 100, 0);
        
        // Обновляем информацию
        UpdateUIInfo();
        
        // Показываем
        _upgradePanel.SetActive(true);
    }
    
    void UpdateUIInfo()
    {
        if (_selectedTower == null) return;
        
        // Информация о продаже
        int sellPrice = _selectedTower.GetSellPrice();
        _sellText.text = $"Sell: {sellPrice}G";
        
        // Информация об улучшениях
        TowerUpgrade upgradeA = _selectedTower.GetUpgradeData(0);
        TowerUpgrade upgradeB = _selectedTower.GetUpgradeData(1);
        
        if (upgradeA != null)
        {
            _upgradePathAText.text = $"{upgradeA.upgradeName}\n{upgradeA.cost}G";
            _upgradePathAButton.interactable = true;
        }
        else
        {
            _upgradePathAText.text = "MAX";
            _upgradePathAButton.interactable = false;
        }
        
        if (upgradeB != null)
        {
            _upgradePathBText.text = $"{upgradeB.upgradeName}\n{upgradeB.cost}G";
            _upgradePathBButton.interactable = true;
        }
        else
        {
            _upgradePathBText.text = "MAX";
            _upgradePathBButton.interactable = false;
        }
    }
    
    void UpgradeTower(int pathIndex)
    {
        if (_selectedPad != null)
        {
            _selectedPad.UpgradeTower(pathIndex);
            UpdateUIInfo(); // Обновляем UI после улучшения
        }
    }
    
    void SellTower()
    {
        if (_selectedPad != null)
        {
            _selectedPad.SellTower();
            HideUI();
        }
    }
    
    public void HideUI()
    {
        _upgradePanel.SetActive(false);
        _selectedTower = null;
        _selectedPad = null;
    }
    
    // Проверяем клик вне UI
    void Update()
    {
        if (_upgradePanel.activeSelf && Input.GetMouseButtonDown(0))
        {
            // Если клик не на UI элементе
            if (!IsPointerOverUIElement())
            {
                HideUI();
            }
        }
    }
    
    bool IsPointerOverUIElement()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}