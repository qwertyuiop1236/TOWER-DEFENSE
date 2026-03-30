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
    // Удаляем ссылку на TowerBuildManager
    // [SerializeField] private TowerBuildManager _buildManager;

    void Start()
    {
        foreach (TowerButtonInfo buttonInfo in _towerButtons)
        {
            if (buttonInfo.button != null && buttonInfo.towerData != null)
            {
                if (buttonInfo.icon != null)
                    buttonInfo.icon.sprite = buttonInfo.towerData.icon;

                if (buttonInfo.costText != null)
                    buttonInfo.costText.text = $"{buttonInfo.towerData.baseCost}G";

                buttonInfo.button.onClick.AddListener(() => SelectTower(buttonInfo.towerData));
            }
        }
    }

    void SelectTower(TowerData towerData)
    {
        BuildManager.EnterBuildMode(towerData);
    }

    void Update()
    {
        foreach (TowerButtonInfo buttonInfo in _towerButtons)
        {
            if (buttonInfo.button != null && buttonInfo.towerData != null)
            {
                bool canAfford = StatsSystem.Instance.Money >= buttonInfo.towerData.baseCost;
                buttonInfo.button.interactable = canAfford;
                if (buttonInfo.costText != null)
                    buttonInfo.costText.color = canAfford ? Color.white : Color.red;
            }
        }
    }
}