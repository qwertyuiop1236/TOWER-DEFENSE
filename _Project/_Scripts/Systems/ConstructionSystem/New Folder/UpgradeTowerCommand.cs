using UnityEngine;
public class UpgradeTowerCommand : ICommand
{
    private readonly BuildPad _buildPad;
    private readonly int _pathIndex;
    private readonly int _upgradeCost;
    private TowerUpgrade _appliedUpgrade;

    public UpgradeTowerCommand(BuildPad buildPad, int pathIndex)
    {
        _buildPad = buildPad;
        _pathIndex = pathIndex;
        TowerUpgrade upgrade = buildPad.CurrentTower?.GetUpgradeData(pathIndex);
        _upgradeCost = upgrade != null ? upgrade.cost : 0;
    }

    public void Execute()
    {
        if (_buildPad.CurrentTower == null) return;
        TowerUpgrade upgrade = _buildPad.CurrentTower.GetUpgradeData(_pathIndex);
        if (upgrade == null) return;

        if (!StatsSystem.Instance.TrySpendMoney(upgrade.cost))
        {
            Debug.Log("Недостаточно денег для улучшения!");
            return;
        }

        bool success = _buildPad.CurrentTower.ApplyUpgrade(upgrade, _pathIndex);
        if (success)
        {
            _appliedUpgrade = upgrade;
            Debug.Log($"Улучшение {upgrade.upgradeName} применено");
        }
        else
        {
            StatsSystem.Instance.AddMoney(upgrade.cost); // возврат
        }
    }

    public void Undo()
    {
        if (_appliedUpgrade != null && _buildPad.CurrentTower != null)
        {
            // Для отката нужен механизм понижения уровня. В текущей реализации Tower не имеет Downgrade.
            // Можно сохранить предыдущее состояние или реализовать обратный расчёт.
            Debug.Log("Откат улучшения требует дополнительной реализации");
            // Вернём деньги (если нужно)
            StatsSystem.Instance.AddMoney(_appliedUpgrade.cost);
        }
    }
}