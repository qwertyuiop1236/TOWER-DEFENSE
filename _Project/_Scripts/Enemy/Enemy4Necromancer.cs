using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNecromancer : Enemy
{
    [Header("настройка уникальных параметров Tower")]
    [SerializeField] protected float speedMuveNecromancer;
    [SerializeField] protected int damageNecromancer;
    [SerializeField] protected float xpNecromancer = 400f;

    [Header("Общие параметры")]
    [SerializeField] protected EnemyDataSO SkeletonData;
    [SerializeField] protected float TimeSpavn = 0;
    [SerializeField] protected float TimeSpavnMax = 1;

    protected override void Start()
    {
        base.Start();
        _moveSpeed = speedMuveNecromancer;
        _damageToBase = damageNecromancer;
        _health = xpNecromancer;
        TimeSpavn = TimeSpavnMax;
    }

    protected override void Update()
    {
        base.Update();
        TimeSpavn -= Time.deltaTime;
        if (TimeSpavn <= 0)
        {
            Spawner();
            TimeSpavn += TimeSpavnMax;
        }
    }

    protected void Spawner()
    {
        // Используем фабрику для создания скелета (вместо Instantiate)
        EnemyFactory.Create(SkeletonData, transform.position, Quaternion.identity);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}