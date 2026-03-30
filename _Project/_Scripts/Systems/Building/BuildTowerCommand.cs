using UnityEngine;

public class BuildTowerCommand : ICommand
{
    private readonly TowerData _towerData;
    private readonly BuildPad _buildPad;
    private readonly int _cost;
    private GameObject _builtTower;

    public BuildTowerCommand(TowerData towerData, BuildPad buildPad)
    {
        _towerData = towerData;
        _buildPad = buildPad;
        _cost = towerData.baseCost;
    }

    public void Execute()
    {
        if (_buildPad.IsOccupied) return;
        if (!StatsSystem.Instance.TrySpendMoney(_cost))
        {
            Debug.Log("Недостаточно денег для постройки!");
            return;
        }

        // Используем пул для создания башни
        GameObject towerObj = ObjectPool.Instance.Get(_towerData.prefab, _buildPad.transform.position, Quaternion.identity);
        Tower tower = towerObj.GetComponent<Tower>();
        if (tower == null)
        {
            Debug.LogError($"Префаб {_towerData.prefab.name} не содержит компонент Tower!");
            ObjectPool.Instance.Return(towerObj);
            StatsSystem.Instance.AddMoney(_cost); // возвращаем деньги
            return;
        }

        tower.Initialize(_towerData, _buildPad);
        _buildPad.SetTower(tower);
        _builtTower = towerObj;

        Debug.Log($"Построена {_towerData.towerName}");
    }

    public void Undo()
    {
        if (_builtTower != null)
        {
            _buildPad.ClearTower();
            ObjectPool.Instance.Return(_builtTower);
            StatsSystem.Instance.AddMoney(_cost);
            Debug.Log("Отмена строительства");
        }
    }
}