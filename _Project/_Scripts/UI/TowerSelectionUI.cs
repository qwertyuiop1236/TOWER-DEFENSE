using UnityEngine;
using UnityEngine.UI;

public class TowerSelectionUI : MonoBehaviour
{
    [System.Serializable]
    public class TowerButtonInfo
    {
        public Button button;
        public TowerData towerData;
        public Image icon;
        public Text costText;
    }
    
    [SerializeField] private TowerButtonInfo[] _towerButtons;
    [SerializeField] private TowerBuildManager _buildManager;
    
    void Start()
    {
        // Инициализируем кнопки
        foreach (TowerButtonInfo buttonInfo in _towerButtons)
        {
            if (buttonInfo.button != null && buttonInfo.towerData != null)
            {
                // Настраиваем внешний вид
                if (buttonInfo.icon != null)
                    buttonInfo.icon.sprite = buttonInfo.towerData.icon;
                    
                if (buttonInfo.costText != null)
                    buttonInfo.costText.text = $"{buttonInfo.towerData.baseCost}G";
                
                // Назначаем обработчик
                buttonInfo.button.onClick.AddListener(() => 
                    SelectTower(buttonInfo.towerData));
            }
        }
    }
    
    void SelectTower(TowerData towerData)
    {
        if (_buildManager != null)
        {
            _buildManager.EnterBuildMode(towerData);
        }
    }
    
    void Update()
    {
        // Обновляем доступность кнопок (серые если не хватает денег)
        foreach (TowerButtonInfo buttonInfo in _towerButtons)
        {
            if (buttonInfo.button != null && buttonInfo.towerData != null)
            {
                bool canAfford = StatsSystem.Instance.Money >= buttonInfo.towerData.baseCost;
                buttonInfo.button.interactable = canAfford;
                
                // Меняем цвет текста стоимости
                if (buttonInfo.costText != null)
                {
                    buttonInfo.costText.color = canAfford ? Color.white : Color.red;
                }
            }
        }
    }
}