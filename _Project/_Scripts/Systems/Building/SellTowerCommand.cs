using UnityEngine;

public class SellTowerCommand : MonoBehaviour, ICommand
{
    private readonly BuildPad _buildPad;
    private int _sellPrice;
    private GameObject _soldTowerObject;

    public SellTowerCommand(BuildPad buildPad)
    {
        _buildPad = buildPad;
    }

    public void Execute()
    {
        if (_buildPad.CurrentTower == null) return;
        _sellPrice = _buildPad.CurrentTower.GetSellPrice();
        _soldTowerObject = _buildPad.CurrentTower.gameObject;
        _buildPad.ClearTower();
        Destroy(_soldTowerObject);  // ← вместо ObjectPool.Return
        StatsSystem.Instance.AddMoney(_sellPrice);
    }

    public void Undo()
    {
        if (_soldTowerObject != null && _buildPad != null)
        {
            Tower tower = _soldTowerObject.GetComponent<Tower>();
            if (tower != null)
            {
                _buildPad.SetTower(tower);
                StatsSystem.Instance.TrySpendMoney(_sellPrice); // убираем деньги
                Debug.Log("Отмена продажи");
            }
        }
    }
}