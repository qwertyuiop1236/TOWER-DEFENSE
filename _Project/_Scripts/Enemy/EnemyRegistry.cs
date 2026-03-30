using System.Collections.Generic;
using UnityEngine;

public static class EnemyRegistry
{
    private static List<Enemy> _allEnemies = new List<Enemy>();

    public static IReadOnlyList<Enemy> AllEnemies => _allEnemies;

    public static void Register(Enemy enemy)
    {
        if (enemy == null) return;
        if (!_allEnemies.Contains(enemy))
            _allEnemies.Add(enemy);
    }

    public static void Unregister(Enemy enemy)
    {
        if (enemy == null) return;
        _allEnemies.Remove(enemy);
    }
}
