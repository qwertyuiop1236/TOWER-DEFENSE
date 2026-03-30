using System.Collections.Generic;
using UnityEngine;

public class StrongestTargetStrategy : ITargetStrategy
{
    public Enemy GetTarget(List<Enemy> enemiesInRange, Vector3 towerPosition)
    {
        Enemy strongest = null;
        float maxHealth = 0f;
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            if (enemy.Health > maxHealth)
            {
                maxHealth = enemy.Health;
                strongest = enemy;
            }
        }
        return strongest;
    }
}