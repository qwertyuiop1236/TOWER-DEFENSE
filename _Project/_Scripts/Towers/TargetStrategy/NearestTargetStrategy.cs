using System.Collections.Generic;
using UnityEngine;

public class NearestTargetStrategy : ITargetStrategy
{
    public Enemy GetTarget(List<Enemy> enemiesInRange, Vector3 towerPosition)
    {
        Enemy closest = null;
        float minDist = float.MaxValue;
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(towerPosition, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }
}