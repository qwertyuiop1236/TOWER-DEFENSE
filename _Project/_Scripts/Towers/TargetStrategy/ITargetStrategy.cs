using System.Collections.Generic;
using UnityEngine;

public interface ITargetStrategy
{
    Enemy GetTarget(List<Enemy> enemiesInRange, Vector3 towerPosition);
}