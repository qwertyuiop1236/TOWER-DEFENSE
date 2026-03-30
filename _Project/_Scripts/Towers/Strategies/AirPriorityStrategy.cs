using System.Collections.Generic;
using UnityEngine;

public class AirPriorityStrategy : ITargetStrategy
{
    private readonly ITargetStrategy _fallbackStrategy;

    public AirPriorityStrategy(ITargetStrategy fallback)
    {
        _fallbackStrategy = fallback;
    }

    public Enemy GetTarget(List<Enemy> enemiesInRange, Vector3 towerPosition)
    {
        // Сначала ищем летающих врагов (предположим, у Enemy есть свойство IsFlying)
        List<Enemy> flying = new List<Enemy>();
        foreach (Enemy enemy in enemiesInRange)
        {
            // if (enemy.IsFlying) flying.Add(enemy);
        }
        if (flying.Count > 0)
            return _fallbackStrategy.GetTarget(flying, towerPosition);
        return _fallbackStrategy.GetTarget(enemiesInRange, towerPosition);
    }
}