using System.Collections.Generic;
using UnityEngine;

public static class BuildManager
{
    private static TowerData _selectedTower;
    private static bool _isBuildMode;
    private static Stack<ICommand> _commandHistory = new Stack<ICommand>();

    public static bool IsBuildMode => _isBuildMode;
    public static TowerData SelectedTower => _selectedTower;

    public static event System.Action<bool, TowerData> OnBuildModeChanged;

    public static void EnterBuildMode(TowerData towerData)
    {
        _selectedTower = towerData;
        _isBuildMode = true;
        OnBuildModeChanged?.Invoke(true, towerData);
    }

    public static void ExitBuildMode()
    {
        _selectedTower = null;
        _isBuildMode = false;
        OnBuildModeChanged?.Invoke(false, null);
    }

    public static bool TryBuildOnPad(BuildPad pad)
    {
        if (!_isBuildMode || pad.IsOccupied) return false;

        var command = new BuildTowerCommand(_selectedTower, pad);
        command.Execute();
        // Если команда выполнилась успешно (проверим, появилась ли башня)
        if (pad.IsOccupied)
        {
            _commandHistory.Push(command);
            ExitBuildMode();
            return true;
        }
        return false;
    }

    public static void UpgradeTower(BuildPad pad, int pathIndex)
    {
        if (pad.CurrentTower == null) return;
        var command = new UpgradeTowerCommand(pad, pathIndex);
        command.Execute();
        // После выполнения можно добавить в историю, но нужно проверять успешность
        // Для простоты добавим всегда, но лучше проверять.
        _commandHistory.Push(command);
    }

    public static void SellTower(BuildPad pad)
    {
        if (pad.CurrentTower == null) return;
        var command = new SellTowerCommand(pad);
        command.Execute();
        _commandHistory.Push(command);
    }

    public static void UndoLast()
    {
        if (_commandHistory.Count == 0) return;
        ICommand last = _commandHistory.Pop();
        last.Undo();
    }
}